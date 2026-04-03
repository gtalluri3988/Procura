using DB.EFModel;
using DB.Entity;
using DB.Model;
using DB.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DB.Repositories
{
    public class BiddingRepository : IBiddingRepository
    {
        private readonly ProcuraDbContext _context;

        public BiddingRepository(ProcuraDbContext context)
        {
            _context = context;
        }

        // ── Vendor portal — list of open tenders available for bidding ────────
        public async Task<List<BiddingListDto>> GetActiveBiddingListAsync(int vendorId)
        {
            var now = DateTime.UtcNow;

            var tenders = await _context.TenderApplications
                .Include(t => t.TenderCategory)
                .ToListAsync();

            var tenderIds = tenders.Select(t => t.Id).ToList();

            var adSettings = await _context.TenderAdvertisementSettings
                .Where(a => tenderIds.Contains(a.TenderId))
                .ToDictionaryAsync(a => a.TenderId);

            return tenders
                .Where(t => adSettings.ContainsKey(t.Id))
                .Select(t =>
                {
                    var ad = adSettings[t.Id];
                    var closingDateTime = ad.AdvertisementEndDate.Date + ad.EndTime;
                    var isOpen = closingDateTime >= now;

                    return new BiddingListDto
                    {
                        TenderId = t.Id,
                        ReferenceId = "T" + t.Id.ToString("D7"),
                        BiddingTitle = t.ProjectName,
                        TenderCategory = t.TenderCategory?.Name,
                        StartingDate = ad.AdvertisementStartDate,
                        ClosingDate = ad.AdvertisementEndDate,
                        ClosingTime = ad.EndTime.ToString(@"hh\:mm") + (ad.EndTime.Hours < 12 ? "AM" : "PM"),
                        Status = isOpen ? "Open" : "Closed"
                    };
                })
                .ToList();
        }

        // ── Vendor portal — full bidding details page ─────────────────────────
        public async Task<BiddingDetailDto?> GetBiddingDetailAsync(int tenderId, int vendorId)
        {
            var tender = await _context.TenderApplications
                .Include(t => t.TenderCategory)
                .Include(t => t.JobCategory)
                .FirstOrDefaultAsync(t => t.Id == tenderId);

            if (tender == null) return null;

            var ad = await _context.TenderAdvertisementSettings
                .FirstOrDefaultAsync(a => a.TenderId == tenderId);

            var siteLevel = tender.ApplicationLevelId.HasValue
                ? await _context.SiteLevel.FirstOrDefaultAsync(s => s.Id == tender.ApplicationLevelId.Value)
                : null;

            // Get assets for this tender
            var assets = await _context.BiddingAssets
                .Where(a => a.TenderId == tenderId)
                .OrderBy(a => a.SequenceNo)
                .ToListAsync();

            // Get existing vendor submission if already bid
            var submission = await _context.TenderVendorSubmissions
                .FirstOrDefaultAsync(s => s.TenderId == tenderId && s.VendorId == vendorId);

            var existingItems = new Dictionary<int, decimal>();
            if (submission != null)
            {
                existingItems = await _context.BidderSubmissionItems
                    .Where(i => i.TenderVendorSubmissionId == submission.Id)
                    .ToDictionaryAsync(i => i.BiddingAssetId, i => i.BidPrice);
            }

            var assetDtos = assets.Select(a =>
            {
                existingItems.TryGetValue(a.Id, out var bidPrice);
                return new BiddingAssetDto
                {
                    Id = a.Id,
                    SequenceNo = a.SequenceNo,
                    ProjectState = a.ProjectState,
                    AssetDetails = a.AssetDetails,
                    AssetRefNo = a.AssetRefNo,
                    StartingPrice = a.StartingPrice,
                    YearPurchased = a.YearPurchased,
                    BidPrice = bidPrice
                };
            }).ToList();

            string? closingTime = null;
            if (ad != null)
                closingTime = ad.EndTime.ToString(@"hh\:mm") + (ad.EndTime.Hours < 12 ? "AM" : "PM");

            return new BiddingDetailDto
            {
                TenderId = tenderId,
                ReferenceId = "T" + tenderId.ToString("D7"),
                ApplicationLevel = siteLevel?.Name,
                BiddingTitle = tender.ProjectName,
                JobCategory = tender.JobCategory?.Name,
                TenderCategory = tender.TenderCategory?.Name,
                DepositAmount = tender.DepositAmount,
                Remarks = tender.Remarks,
                StartingDate = ad?.AdvertisementStartDate,
                ClosingDate = ad?.AdvertisementEndDate,
                ClosingTime = closingTime,
                AlreadySubmitted = submission != null,
                Assets = assetDtos
            };
        }

        // ── Vendor portal — submit bidding ────────────────────────────────────
        public async Task SubmitBiddingAsync(SubmitBiddingDto dto)
        {
            // Upsert TenderVendorSubmission (header)
            var submission = await _context.TenderVendorSubmissions
                .FirstOrDefaultAsync(s => s.TenderId == dto.TenderId && s.VendorId == dto.VendorId);

            if (submission == null)
            {
                // Assign next sequence number
                var maxSeq = await _context.TenderVendorSubmissions
                    .Where(s => s.TenderId == dto.TenderId)
                    .MaxAsync(s => (int?)s.SequenceNo) ?? 0;

                submission = new TenderVendorSubmission
                {
                    TenderId = dto.TenderId,
                    VendorId = dto.VendorId,
                    SequenceNo = maxSeq + 1,
                    TenderOpeningStatus = "Pending"
                };
                _context.TenderVendorSubmissions.Add(submission);
                await _context.SaveChangesAsync();
            }

            // Remove old bid items and re-insert
            var oldItems = _context.BidderSubmissionItems
                .Where(i => i.TenderVendorSubmissionId == submission.Id);
            _context.BidderSubmissionItems.RemoveRange(oldItems);

            var newItems = dto.BidItems.Select(b => new BidderSubmissionItem
            {
                TenderVendorSubmissionId = submission.Id,
                BiddingAssetId = b.BiddingAssetId,
                BidPrice = b.BidPrice
            }).ToList();

            _context.BidderSubmissionItems.AddRange(newItems);

            // Update total offered price = sum of all bid prices
            submission.OfferedPrice = dto.BidItems.Sum(b => b.BidPrice);

            await _context.SaveChangesAsync();
        }

        // ── Vendor portal — Award Details tab ─────────────────────────────────
        public async Task<BiddingAwardDetailDto?> GetBiddingAwardDetailAsync(int tenderId, int vendorId)
        {
            var tender = await _context.TenderApplications
                .FirstOrDefaultAsync(t => t.Id == tenderId);

            if (tender == null) return null;

            var award = await _context.TenderAwards
                .Include(a => a.AwardedVendor)
                .FirstOrDefaultAsync(a => a.TenderId == tenderId);

            bool isAwarded = award?.AwardedVendorId == vendorId;

            var ack = await _context.BidderAcknowledgements
                .FirstOrDefaultAsync(a => a.TenderId == tenderId && a.VendorId == vendorId);

            return new BiddingAwardDetailDto
            {
                TenderId = tenderId,
                ReferenceId = "T" + tenderId.ToString("D7"),
                BidderName = award?.AwardedVendor?.CompanyName,
                BiddingTitle = tender.ProjectName,
                IsAwarded = isAwarded,
                AwardDate = award?.AgreementDateSigned,
                Acknowledgement = ack?.Acknowledgement,
                StampDutyPath = ack?.StampDutyPath,
                StampDutyFileName = ack?.StampDutyFileName,
                AcknowledgementDateTime = ack?.AcknowledgementDateTime,
                AlreadyAcknowledged = ack != null
            };
        }

        // ── Vendor portal — Submit acknowledgement (Award Details → Submit) ───
        public async Task SubmitBidderAcknowledgementAsync(SubmitBidderAcknowledgementDto dto)
        {
            var existing = await _context.BidderAcknowledgements
                .FirstOrDefaultAsync(a => a.TenderId == dto.TenderId && a.VendorId == dto.VendorId);

            if (existing != null)
            {
                existing.Acknowledgement = dto.Acknowledgement;
                existing.StampDutyPath = dto.StampDutyPath;
                existing.StampDutyFileName = dto.StampDutyFileName;
                existing.AcknowledgementDateTime = DateTime.UtcNow;
            }
            else
            {
                _context.BidderAcknowledgements.Add(new BidderAcknowledgement
                {
                    TenderId = dto.TenderId,
                    VendorId = dto.VendorId,
                    Acknowledgement = dto.Acknowledgement,
                    StampDutyPath = dto.StampDutyPath,
                    StampDutyFileName = dto.StampDutyFileName,
                    AcknowledgementDateTime = DateTime.UtcNow
                });
            }

            await _context.SaveChangesAsync();
        }

        // ── Admin — get bidding assets for a tender ───────────────────────────
        public async Task<List<BiddingAssetDto>> GetBiddingAssetsAsync(int tenderId)
        {
            return await _context.BiddingAssets
                .Where(a => a.TenderId == tenderId)
                .OrderBy(a => a.SequenceNo)
                .Select(a => new BiddingAssetDto
                {
                    Id = a.Id,
                    SequenceNo = a.SequenceNo,
                    ProjectState = a.ProjectState,
                    AssetDetails = a.AssetDetails,
                    AssetRefNo = a.AssetRefNo,
                    StartingPrice = a.StartingPrice,
                    YearPurchased = a.YearPurchased,
                    BidPrice = 0
                })
                .ToListAsync();
        }

        // ── Admin — save (add/edit) bidding asset ─────────────────────────────
        public async Task SaveBiddingAssetAsync(SaveBiddingAssetDto dto)
        {
            BiddingAsset asset;

            if (dto.Id > 0)
            {
                asset = await _context.BiddingAssets.FindAsync(dto.Id)
                    ?? throw new KeyNotFoundException($"Bidding asset {dto.Id} not found");
            }
            else
            {
                asset = new BiddingAsset { TenderId = dto.TenderId };
                _context.BiddingAssets.Add(asset);
            }

            asset.SequenceNo = dto.SequenceNo;
            asset.ProjectState = dto.ProjectState;
            asset.AssetDetails = dto.AssetDetails;
            asset.AssetRefNo = dto.AssetRefNo;
            asset.StartingPrice = dto.StartingPrice;
            asset.YearPurchased = dto.YearPurchased;

            await _context.SaveChangesAsync();
        }

        // ── Admin — delete bidding asset ──────────────────────────────────────
        public async Task DeleteBiddingAssetAsync(int assetId)
        {
            var asset = await _context.BiddingAssets.FindAsync(assetId);
            if (asset == null) return;

            _context.BiddingAssets.Remove(asset);
            await _context.SaveChangesAsync();
        }

        // ── Tender Opening — Verify ───────────────────────────────────────────
        public async Task VerifyTenderOpeningAsync(VerifyTenderOpeningDto dto, int userId)
        {
            // Check if already verified
            var existing = await _context.TenderOpeningVerifications
                .FirstOrDefaultAsync(v => v.TenderId == dto.TenderId);

            if (existing != null)
            {
                existing.VerifiedByUserId = userId;
                existing.VerifiedAt = DateTime.UtcNow;
                existing.Remarks = dto.Remarks;
            }
            else
            {
                _context.TenderOpeningVerifications.Add(new TenderOpeningVerification
                {
                    TenderId = dto.TenderId,
                    VerifiedByUserId = userId,
                    VerifiedAt = DateTime.UtcNow,
                    Remarks = dto.Remarks
                });
            }

            // Update all vendor submission statuses to Passed (opening verified)
            var submissions = await _context.TenderVendorSubmissions
                .Where(s => s.TenderId == dto.TenderId)
                .ToListAsync();

            foreach (var s in submissions)
                s.TenderOpeningStatus = "Passed";

            await _context.SaveChangesAsync();
        }

        // ── Tender Opening — Progress tab ─────────────────────────────────────
        public async Task<TenderOpeningProgressDto?> GetTenderOpeningProgressAsync(int tenderId)
        {
            var tender = await _context.TenderApplications
                .FirstOrDefaultAsync(t => t.Id == tenderId);

            if (tender == null) return null;

            var verification = await _context.TenderOpeningVerifications
                .Include(v => v.VerifiedByUser)
                .FirstOrDefaultAsync(v => v.TenderId == tenderId);

            var submissions = await _context.TenderVendorSubmissions
                .Include(s => s.Vendor)
                .Where(s => s.TenderId == tenderId)
                .OrderBy(s => s.SequenceNo)
                .ToListAsync();

            var bidProgress = submissions.Select((s, idx) => new TenderOpeningBidProgressDto
            {
                Bil = idx + 1,
                VendorName = s.Vendor?.CompanyName,
                OfferedPrice = s.OfferedPrice,
                OpeningStatus = s.TenderOpeningStatus
            }).ToList();

            return new TenderOpeningProgressDto
            {
                TenderId = tenderId,
                ReferenceId = "T" + tenderId.ToString("D7"),
                ProjectName = tender.ProjectName,
                OpeningStatus = verification != null ? "Verified" : "Pending",
                VerifiedAt = verification?.VerifiedAt,
                VerifiedByName = verification?.VerifiedByUser?.FullName,
                Remarks = verification?.Remarks,
                TotalBidsReceived = submissions.Count,
                BidProgress = bidProgress
            };
        }
    }
}
