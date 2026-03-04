using AutoMapper;
using AutoMapper;
using Azure;
using Azure.Core;
using DB.EFModel;
using DB.EFModel;
using DB.Entity;
using DB.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace DB.Repositories
{
    public class MicrobitRepository : RepositoryBase<VisitorAccessDetails, VisitorAccessDetailsDTO>, IMicrobitRepository
    {
        public MicrobitRepository(CSADbContext context, IMapper mapper, IHttpContextAccessor httpContextAccessor) : base(context, mapper, httpContextAccessor) { }
        public async Task<List<VehicleProfile>> GetMaintenanceStatusAsync(string communityId)
        {
            ConvertRequestObjectToJson(communityId);
            var comId=await _context.Community.Where(x=>x.CommunityId==communityId).FirstOrDefaultAsync();
            if (comId == null)
                throw new Exception("No community found");
            // Replace with real query or stored procedure call
            return await _context.VehicleDetails.Include(x=>x.Resident)
                .Where( v=>v.Resident!=null && v.Resident.CommunityId == comId.Id)
                .Select(v => new VehicleProfile
                {
                    VehiclePlateNo = v.VehicleNo??"",
                    MaintenanceBillStatus = "Paid",
                   
                }).ToListAsync();
        }

        //public async Task<bool> UpdateVehicleEntryExitAsync(VehicleEntryExitRequest request)
        //{
        //    try
        //    {
        //        // Log request for debugging
        //        ConvertRequestObjectToJson(request);

        //        // 1️⃣ Validate community
        //        var community = await _context.Community
        //            .FirstOrDefaultAsync(x => x.CommunityId == request.CommunityId);
        //        if (community == null)
        //            throw new Exception("No community found");

        //        // 2️⃣ Validate vehicle
        //        //var vehicle = await _context.VehicleDetails
        //        //    .FirstOrDefaultAsync(x => x.VehicleNo == request.VehiclePlateNo);
        //        //if (vehicle == null)
        //        //    throw new Exception("No vehicle found");

        //        // 3️⃣ Check VisitorAccessDetails first
        //        var visitor = await _context.VisitorAccessDetails
        //            .FirstOrDefaultAsync(x => x.VehicleNo.Replace(" ", "") == request.VehiclePlateNo.Replace(" ", "") && x.CommunityId== community.Id);

        //        if (visitor != null)
        //        {
        //            if(visitor.EntryTime!=null && visitor.ExitTime != null)
        //            {
        //                visitor.ExitTime = request.ExitTime;
        //                _context.VisitorAccessDetails.Add(visitor);
        //                await _context.SaveChangesAsync();
        //                return true;
        //            }
        //            // If EntryTime is null, it's a new entry; otherwise, update exit
        //            if (visitor.EntryTime == null)
        //            {
        //                visitor.EntryTime = request.EntryTime;
        //            }

        //            visitor.ExitTime = request.ExitTime;
        //            _context.VisitorAccessDetails.Update(visitor);
        //            await _context.SaveChangesAsync();
        //            return true;
        //        }

        //        // 4️⃣ Else check ResidentAccessHistory
        //        var Resident = await _context.VehicleDetails.Include(x=>x.Resident).
        //            Where(x=>x.VehicleNo==request.VehiclePlateNo && x.Resident.CommunityId==community.Id).FirstOrDefaultAsync();
        //        if (Resident != null)
        //        {
        //            var history = await _context.ResidentAccessHistory
        //                .FirstOrDefaultAsync(x => x.VehicleNo == request.VehiclePlateNo && x.CommunityId== community.Id);
        //            if (history == null)
        //            {
        //                var newHistory = new ResidentAccessHistory
        //                {
        //                    VehicleNo = request.VehiclePlateNo,
        //                    CommunityId = community.Id,
        //                    ResidentId = Resident?.ResidentId,
        //                    EntryTime = request.EntryTime,
        //                    ExitTime = request.ExitTime
        //                };
        //                _context.ResidentAccessHistory.Add(newHistory);
        //            }
        //            else
        //            {
        //                history.ExitTime = request.ExitTime;
        //                _context.ResidentAccessHistory.Update(history);
        //            }
        //        }


        //        await _context.SaveChangesAsync();
        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        // Ideally log the exception here
        //        Console.WriteLine($"Error updating vehicle entry/exit: {ex.Message}");
        //        return false;
        //    }
        //}

        //public async Task<bool> UpdateVehicleEntryExitAsync(VehicleEntryExitRequest request)
        //{
        //    try
        //    {
        //        ConvertRequestObjectToJson(request);

        //        // Validate community
        //        var community = await _context.Community
        //            .FirstOrDefaultAsync(x => x.CommunityId == request.CommunityId);

        //        if (community == null)
        //            throw new Exception("No community found");

        //        string plate = request.VehiclePlateNo.Replace(" ", "");

        //        //--------------------------------------------------------------------
        //        // 1️⃣ CHECK VISITOR VEHICLE
        //        //--------------------------------------------------------------------
        //        var visitor = await _context.VisitorAccessDetails
        //            .Where(x => x.VehicleNo.Replace(" ", "") == plate &&
        //                        x.CommunityId == community.Id)
        //            .OrderByDescending(x => x.Id)
        //            .FirstOrDefaultAsync();

        //        if (visitor != null && string.IsNullOrEmpty(request.ImageId))
        //        {
        //            // Check if an open entry exists (ExitTime = null)
        //            var openVisitor = await _context.VisitorAccessDetails
        //                .Where(x => x.VehicleNo.Replace(" ", "") == plate &&
        //                            x.CommunityId == community.Id &&
        //                            x.ExitTime == null)
        //                .OrderByDescending(x => x.Id)
        //                .FirstOrDefaultAsync();

        //            var otherEntry= await _context.VisitorAccessDetails
        //                .Where(x => x.VehicleNo.Replace(" ", "") == plate &&
        //                            x.CommunityId == community.Id )

        //                .OrderByDescending(x => x.Id)
        //                .FirstOrDefaultAsync();
        //            if(otherEntry.EntryTime!=null && otherEntry.ExitTime!=null)
        //            {



        //                var newVisitor = new VisitorAccessDetails
        //                {
        //                    VisitorName=otherEntry.VisitorName,
        //                    VisitorAccessTypeId=otherEntry.VisitorAccessTypeId,
        //                    VisitDate=otherEntry.VisitDate,
        //                    VisitorAccessType=otherEntry.VisitorAccessType,
        //                    VisitPurpose=otherEntry.VisitPurpose,
        //                    CreatedDate=DateTime.Now,
        //                    ResidentId=otherEntry.ResidentId,

        //                    VehicleNo = request.VehiclePlateNo,
        //                    CommunityId = community.Id,
        //                    EntryTime = request.EntryTime,
        //                    ExitTime = request.ExitTime,
        //                };

        //                _context.VisitorAccessDetails.Add(newVisitor);

        //                await _context.SaveChangesAsync();
        //                return true;


        //            }

        //            if (openVisitor == null)
        //            {
        //                // Leaving → update exit time
        //                openVisitor.ExitTime = request.ExitTime;
        //                _context.VisitorAccessDetails.Update(openVisitor);
        //            }
        //            else
        //            {
        //                // New entry → create new record
        //                //var newVisitor = new VisitorAccessDetails
        //                //{
        //                //    VehicleNo = request.VehiclePlateNo,
        //                //    CommunityId = community.Id,
        //                //    EntryTime = request.EntryTime,
        //                //    ExitTime = null
        //                //};

        //                //_context.VisitorAccessDetails.Add(newVisitor);
        //                openVisitor.ExitTime = request.ExitTime;
        //                openVisitor.EntryTime = request.EntryTime;
        //                _context.VisitorAccessDetails.Update(openVisitor);
        //            }

        //            await _context.SaveChangesAsync();
        //            return true;
        //        }

        //        //--------------------------------------------------------------------
        //        // 2️⃣ CHECK RESIDENT VEHICLE
        //        //--------------------------------------------------------------------
        //        var residentVehicle = await _context.VehicleDetails
        //                            .Include(x => x.Resident)
        //                            .Where(x => x.VehicleNo.Replace(" ", "") == plate &&
        //                                        x.Resident.CommunityId == community.Id)
        //                            .FirstOrDefaultAsync();

        //        if (!string.IsNullOrEmpty(request.ImageId))
        //        {
        //            int imgId = Convert.ToInt32(request.ImageId);

        //            residentVehicle = await _context.VehicleDetails
        //                .Include(x => x.Resident)
        //                .Where(x => x.Resident.ResidentPhotos.Any(p => p.Id == imgId) &&
        //                            x.Resident.CommunityId == community.Id)
        //                .FirstOrDefaultAsync();
        //        }


        //        if (residentVehicle != null)
        //        {
        //            // Check if an open entry exists (ExitTime = null)
        //            var openHistory = await _context.ResidentAccessHistory
        //                .Where(x => x.VehicleNo.Replace(" ", "") == plate &&
        //                            x.CommunityId == community.Id &&
        //                            x.ExitTime == null)
        //                .OrderByDescending(x => x.Id)
        //                .FirstOrDefaultAsync();
        //            if (!string.IsNullOrEmpty(request.ImageId))
        //            {
        //                int imgId = Convert.ToInt32(request.ImageId);
        //                openHistory = await _context.ResidentAccessHistory
        //                    .Include(x => x.Resident)
        //                .Where(x => x.Resident.ResidentPhotos.Any(p => p.Id == imgId) &&
        //                            x.CommunityId == community.Id &&
        //                            x.ExitTime == null)
        //                .OrderByDescending(x => x.Id)
        //                .FirstOrDefaultAsync();
        //            }

        //            if (openHistory != null)
        //            {
        //                // Leaving → update exit time
        //                openHistory.ExitTime = request.ExitTime;
        //                _context.ResidentAccessHistory.Update(openHistory);
        //            }
        //            else
        //            {
        //                // Entering → new history record
        //                var newHistory = new ResidentAccessHistory
        //                {
        //                    VehicleNo =string.IsNullOrEmpty(request.VehiclePlateNo)?openHistory?.Resident?.Name: request.VehiclePlateNo,
        //                    CommunityId = community.Id,
        //                    ResidentId = residentVehicle.ResidentId,
        //                    EntryTime = request.EntryTime,
        //                    ExitTime = null
        //                };

        //                _context.ResidentAccessHistory.Add(newHistory);
        //            }

        //            await _context.SaveChangesAsync();
        //            return true;
        //        }

        //        return false; // No visitor, no resident found
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"Error updating vehicle entry/exit: {ex.Message}");
        //        return false;
        //    }
        //}


        //public async Task<bool> UpdateVehicleEntryExitAsync(VehicleEntryExitRequest request)
        //{
        //    try
        //    {
        //        ConvertRequestObjectToJson(request);

        //        var community = await _context.Community
        //            .FirstOrDefaultAsync(x => x.CommunityId == request.CommunityId);

        //        if (community == null)
        //            throw new Exception("No community found");

        //        string plate = request.VehiclePlateNo.Replace(" ", "");

        //        //--------------------------------------------------------------------
        //        // 1️⃣ VISITOR VEHICLE
        //        //--------------------------------------------------------------------
        //        var visitor = await _context.VisitorAccessDetails
        //            .Where(x => x.VehicleNo.Replace(" ", "") == plate &&
        //                        x.CommunityId == community.Id)
        //            .OrderByDescending(x => x.Id)
        //            .FirstOrDefaultAsync();

        //        if (visitor != null && string.IsNullOrEmpty(request.ImageId))
        //        {
        //            var openVisitor = await _context.VisitorAccessDetails
        //                .Where(x => x.VehicleNo.Replace(" ", "") == plate &&
        //                            x.CommunityId == community.Id &&
        //                            x.ExitTime == null)
        //                .OrderByDescending(x => x.Id)
        //                .FirstOrDefaultAsync();

        //            // 🚫 ENTRY already exists → reject second ENTRY
        //            if (openVisitor != null &&
        //                request.EntryTime != null &&
        //                request.ExitTime == null)
        //            {
        //                return true; // reject duplicate ENTRY
        //            }

        //            var otherEntry = await _context.VisitorAccessDetails
        //                .Where(x => x.VehicleNo.Replace(" ", "") == plate &&
        //                            x.CommunityId == community.Id)
        //                .OrderByDescending(x => x.Id)
        //                .FirstOrDefaultAsync();

        //            if (otherEntry.EntryTime != null && otherEntry.ExitTime != null)
        //            {
        //                var newVisitor = new VisitorAccessDetails
        //                {
        //                    VisitorName = otherEntry.VisitorName,
        //                    VisitorAccessTypeId = otherEntry.VisitorAccessTypeId,
        //                    VisitDate = otherEntry.VisitDate,
        //                    VisitorAccessType = otherEntry.VisitorAccessType,
        //                    VisitPurpose = otherEntry.VisitPurpose,
        //                    CreatedDate = DateTime.Now,
        //                    ResidentId = otherEntry.ResidentId,
        //                    VehicleNo = request.VehiclePlateNo,
        //                    CommunityId = community.Id,
        //                    EntryTime = request.EntryTime,
        //                    ExitTime = request.ExitTime,
        //                };

        //                _context.VisitorAccessDetails.Add(newVisitor);
        //                await _context.SaveChangesAsync();
        //                return true;
        //            }

        //            if (openVisitor == null)
        //            {
        //                openVisitor.ExitTime = request.ExitTime;
        //                _context.VisitorAccessDetails.Update(openVisitor);
        //            }
        //            else
        //            {
        //                openVisitor.ExitTime = request.ExitTime;
        //                openVisitor.EntryTime = request.EntryTime;
        //                _context.VisitorAccessDetails.Update(openVisitor);
        //            }

        //            await _context.SaveChangesAsync();
        //            return true;
        //        }

        //        //--------------------------------------------------------------------
        //        // 2️⃣ RESIDENT VEHICLE
        //        //--------------------------------------------------------------------
        //        var residentVehicle = await _context.VehicleDetails
        //            .Include(x => x.Resident)
        //            .Where(x => x.VehicleNo.Replace(" ", "") == plate &&
        //                        x.Resident.CommunityId == community.Id)
        //            .FirstOrDefaultAsync();

        //        if (!string.IsNullOrEmpty(request.ImageId))
        //        {
        //            int imgId = Convert.ToInt32(request.ImageId);

        //            residentVehicle = await _context.VehicleDetails
        //                .Include(x => x.Resident)
        //                .Where(x => x.Resident.ResidentPhotos.Any(p => p.Id == imgId) &&
        //                            x.Resident.CommunityId == community.Id)
        //                .FirstOrDefaultAsync();
        //        }

        //        if (residentVehicle != null)
        //        {
        //            var openHistory = await _context.ResidentAccessHistory
        //                .Where(x => x.VehicleNo.Replace(" ", "") == plate &&
        //                            x.CommunityId == community.Id &&
        //                            x.ExitTime == null)
        //                .OrderByDescending(x => x.Id)
        //                .FirstOrDefaultAsync();

        //            if (!string.IsNullOrEmpty(request.ImageId))
        //            {
        //                int imgId = Convert.ToInt32(request.ImageId);

        //                openHistory = await _context.ResidentAccessHistory
        //                    .Include(x => x.Resident)
        //                    .Where(x => x.Resident.ResidentPhotos.Any(p => p.Id == imgId) &&
        //                                x.CommunityId == community.Id &&
        //                                x.ExitTime == null)
        //                    .OrderByDescending(x => x.Id)
        //                    .FirstOrDefaultAsync();
        //            }

        //            // 🚫 Reject second ENTRY for resident
        //            if (openHistory != null &&
        //                request.EntryTime != null &&
        //                request.ExitTime == null)
        //            {
        //                return true;
        //            }

        //            if (openHistory != null)
        //            {
        //                openHistory.ExitTime = request.ExitTime;
        //                _context.ResidentAccessHistory.Update(openHistory);
        //            }
        //            else
        //            {
        //                var newHistory = new ResidentAccessHistory
        //                {
        //                    VehicleNo = string.IsNullOrEmpty(request.VehiclePlateNo)
        //                        ? openHistory?.Resident?.Name
        //                        : request.VehiclePlateNo,
        //                    CommunityId = community.Id,
        //                    ResidentId = residentVehicle.ResidentId,
        //                    EntryTime = request.EntryTime,
        //                    ExitTime = null
        //                };

        //                _context.ResidentAccessHistory.Add(newHistory);
        //            }

        //            await _context.SaveChangesAsync();
        //            return true;
        //        }

        //        return false;
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"Error updating vehicle entry/exit: {ex.Message}");
        //        return false;
        //    }
        //}


        public async Task<bool> UpdateVehicleEntryExitAsync(VehicleEntryExitRequest request)
        {
            try
            {
                ConvertRequestObjectToJson(request);

                var community = await _context.Community
                    .FirstOrDefaultAsync(x => x.CommunityId == request.CommunityId);

                if (community == null)
                    throw new Exception("No community found");

                string plate = request.VehiclePlateNo?.Replace(" ", "");

                // =====================================================
                // 1️⃣ VISITOR VEHICLE FLOW
                // =====================================================
                var visitor = await _context.VisitorAccessDetails
                    .FirstOrDefaultAsync(x =>
                        x.VehicleNo.Replace(" ", "") == plate &&
                        x.CommunityId == community.Id);

                // Create VISITOR MASTER on ENTRY if not exists
                if (visitor == null && request.EntryTime != null && request.ExitTime == null)
                {
                    //visitor = new VisitorAccessDetails
                    //{
                    //    VisitorName = request.VisitorName,
                    //    VehicleNo = request.VehiclePlateNo,
                    //    VisitPurpose = request.VisitPurpose,
                    //    CommunityId = community.Id,
                    //    VisitDate = DateTime.Today,
                    //    CreatedDate = DateTime.Now
                    //};

                    //_context.VisitorAccessDetails.Add(visitor);
                    //await _context.SaveChangesAsync();
                }

                if (visitor != null)
                {
                    var openTxn = await _context.VisitorDetailsTransactions
                        .Where(x => x.VisitorId == visitor.Id && x.ExitTime == null)
                        .OrderByDescending(x => x.Id)
                        .FirstOrDefaultAsync();

                    // 🚫 Duplicate ENTRY
                    if (openTxn != null && request.EntryTime != null && request.ExitTime == null)
                        return true;

                    // 🔴 EXIT
                    if (openTxn != null && request.ExitTime != null)
                    {
                        openTxn.ExitTime = request.ExitTime;
                        openTxn.UpdatedDate = DateTime.Now;

                        _context.VisitorDetailsTransactions.Update(openTxn);
                        await _context.SaveChangesAsync();
                        return true;
                    }

                    // 🟢 ENTRY
                    if (openTxn == null && request.EntryTime != null)
                    {
                        var txn = new VisitorDetailsTransaction
                        {
                            VisitorId = visitor.Id,
                            EntryTime = request.EntryTime,
                            ExitTime = null,
                            CreatedDate = DateTime.Now
                        };

                        _context.VisitorDetailsTransactions.Add(txn);
                        await _context.SaveChangesAsync();
                        return true;
                    }
                }

                // =====================================================
                // 2️⃣ RESIDENT VEHICLE FLOW
                // =====================================================
                var residentVehicle = await _context.VehicleDetails
                    .Include(x => x.Resident)
                    .FirstOrDefaultAsync(x =>
                        x.VehicleNo.Replace(" ", "") == plate &&
                        x.Resident.CommunityId == community.Id);

                if (residentVehicle == null)
                    return false;

                var openHistory = await _context.ResidentAccessHistory
                    .Where(x =>
                        x.VehicleNo.Replace(" ", "") == plate &&
                        x.CommunityId == community.Id &&
                        x.ExitTime == null)
                    .OrderByDescending(x => x.Id)
                    .FirstOrDefaultAsync();

                // 🚫 Duplicate ENTRY
                if (openHistory != null && request.EntryTime != null && request.ExitTime == null)
                    return true;

                // 🔴 EXIT
                if (openHistory != null && request.ExitTime != null)
                {
                    openHistory.ExitTime = request.ExitTime;
                    openHistory.UpdatedDate = DateTime.Now;

                    _context.ResidentAccessHistory.Update(openHistory);
                    await _context.SaveChangesAsync();
                    return true;
                }

                // 🟢 ENTRY
                if (openHistory == null && request.EntryTime != null)
                {
                    var history = new ResidentAccessHistory
                    {
                        VehicleNo = request.VehiclePlateNo,
                        CommunityId = community.Id,
                        ResidentId = residentVehicle.ResidentId,
                        EntryTime = request.EntryTime,
                        ExitTime = null,
                        CreatedDate = DateTime.Now
                    };

                    _context.ResidentAccessHistory.Add(history);
                    await _context.SaveChangesAsync();
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating vehicle entry/exit: {ex.Message}");
                return false;
            }
        }






        //public async Task<bool> UpdateVehicleEntryExitAsync(VehicleEntryExitRequest request)
        //{
        //    try
        //    {
        //        ConvertRequestObjectToJson(request);

        //        var community = await _context.Community
        //            .FirstOrDefaultAsync(x => x.CommunityId == request.CommunityId);
        //        if (community == null)
        //            throw new Exception("No community found");

        //        // Visitor check
        //        var visitor = await _context.VisitorAccessDetails
        //            .FirstOrDefaultAsync(x => x.VehicleNo == request.VehiclePlateNo && x.CommunityId == community.Id);

        //        if (visitor != null)
        //        {
        //            if (visitor.EntryTime == null)
        //                visitor.EntryTime = request.EntryTime;

        //            visitor.ExitTime = request.ExitTime;
        //            _context.VisitorAccessDetails.Update(visitor);
        //            await _context.SaveChangesAsync();
        //            return true;
        //        }

        //        // Resident check
        //        var residentVehicle = await _context.VehicleDetails
        //            .Include(x => x.Resident)
        //            .FirstOrDefaultAsync(x => x.VehicleNo == request.VehiclePlateNo && x.Resident.CommunityId == community.Id);

        //        if (residentVehicle != null)
        //        {
        //            var history = await _context.ResidentAccessHistory
        //                .FirstOrDefaultAsync(x => x.VehicleNo == request.VehiclePlateNo && x.CommunityId == community.Id);

        //            if (history == null)
        //            {
        //                _context.ResidentAccessHistory.Add(new ResidentAccessHistory
        //                {
        //                    VehicleNo = request.VehiclePlateNo,
        //                    CommunityId = community.Id,
        //                    ResidentId = residentVehicle.ResidentId,
        //                    EntryTime = request.EntryTime,
        //                    ExitTime = request.ExitTime
        //                });
        //            }
        //            else
        //            {
        //                if (history.EntryTime == null)
        //                    history.EntryTime = request.EntryTime;

        //                history.ExitTime = request.ExitTime;
        //                _context.ResidentAccessHistory.Update(history);
        //            }

        //            await _context.SaveChangesAsync();
        //            return true;
        //        }

        //        // Nothing updated
        //        return false;
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"Error updating vehicle entry/exit: {ex.Message}");
        //        return false;
        //    }
        //}


        //public async Task<bool> UpdateVehicleEntryExitAsync(VehicleEntryExitRequest request)
        //{
        //    try
        //    {
        //        ConvertRequestObjectToJson(request);
        //        var comId = await _context.Community.Where(x => x.CommunityId == request.CommunityId).FirstOrDefaultAsync();
        //        if (comId == null)
        //            throw new Exception("No community found");

        //        var vehicle = await _context.VehicleDetails.Where(x => x.VehicleNo == request.VehiclePlateNo).FirstOrDefaultAsync();
        //        if (vehicle == null)
        //            throw new Exception("No vehicle found");

        //        var checkRes=await _context.VehicleDetails.Where(x => x.VehicleNo == request.VehiclePlateNo 
        //        ).FirstOrDefaultAsync();
        //        if (checkRes == null)
        //        {
        //            var checkvisitor = await _context.VisitorAccessDetails
        //                .Where(x => x.VehicleNo == request.VehiclePlateNo
        //                ).FirstOrDefaultAsync();
        //            if (checkvisitor != null)
        //            {
        //                if (checkvisitor.EntryTime == null)
        //                {
        //                    checkvisitor.EntryTime = request.EntryTime;
        //                    checkvisitor.ExitTime = request.ExitTime;
        //                    _context.VisitorAccessDetails.Update(checkvisitor);
        //                    await _context.SaveChangesAsync();
        //                    return true;
        //                }
        //                else
        //                {
        //                                               checkvisitor.ExitTime = request.ExitTime;
        //                    _context.VisitorAccessDetails.Update(checkvisitor);
        //                    await _context.SaveChangesAsync();
        //                    return true;
        //                }
        //            }


        //        }
        //        else
        //        {
        //            var checkHistory = await _context.ResidentAccessHistory
        //                .Where(x => x.VehicleNo == request.VehiclePlateNo 
        //                ).FirstOrDefaultAsync();
        //            if (checkHistory == null)
        //            {
        //                ResidentAccessHistory updateRecord = new ResidentAccessHistory
        //                {
        //                    VehicleNo = request.VehiclePlateNo,
        //                    CommunityId = comId.Id,
        //                    EntryTime = request.EntryTime,
        //                    ExitTime = request.ExitTime
        //                };
        //                _context.ResidentAccessHistory.Add(updateRecord);
        //                await _context.SaveChangesAsync();
        //                return true;
        //            }   
        //            else
        //            {
        //                checkHistory.ExitTime = request.ExitTime;
        //                _context.ResidentAccessHistory.Update(checkHistory);
        //                await _context.SaveChangesAsync();
        //                return true;
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {

        //        return false;
        //    }
        //}

        // Resident – Facial Recognition
        public async Task<List<ResidentFaceProfile>> GetResidentFaceStatusAsync(string communityId)
        {
            ConvertRequestObjectToJson(communityId);

            var residents = await _context.Resident
                .Include(r => r.Community)
                .Include(r => r.ResidentPhotos)
                .Where(r => r.Community != null && r.Community.CommunityId == communityId)
                .Select(r => new
                {
                    r.Id,
                    r.Name,
                    r.CommunityId,
                    r.HouseNo,
                    r.Level,
                    r.BlockNo,
                    r.RoadNo,
                    ResidentPhotos = r.ResidentPhotos.Select(p => new
                    {
                        p.Id,
                        p.ImagePath,
                        p.ImageLabel
                    }).ToList()
                })
                .ToListAsync();

            var result = new List<ResidentFaceProfile>();

            foreach (var resident in residents)
            {
                var MaintanancePaymentStatus = _context.MaintanencePaymentStatus.Where(x => x.ResidentId == resident.Id).OrderByDescending(x=>x.CreatedDate).FirstOrDefault();
                var images = new List<ResidentImage>();

                foreach (var photo in resident.ResidentPhotos)
                {
                    string? base64 = null;

                    if (!string.IsNullOrEmpty(photo.ImagePath) && File.Exists(photo.ImagePath))
                    {
                        try
                        {
                            var bytes = await File.ReadAllBytesAsync(photo.ImagePath);
                            var ext = Path.GetExtension(photo.ImagePath).ToLowerInvariant();
                            var mimeType = ext switch
                            {
                                ".png" => "image/png",
                                ".jpg" or ".jpeg" => "image/jpeg",
                                _ => "application/octet-stream"
                            };
                            base64 = $"data:{mimeType};base64,{Convert.ToBase64String(bytes)}";
                        }
                        catch
                        {
                            // keep base64 as null on error (could log)
                            base64 = null;
                        }
                    }

                    images.Add(new ResidentImage
                    {
                        ResidentImageId = photo.Id,
                        FaceImageBase64 = base64,
                        Name=photo.ImageLabel,
                        UnitNo = $"{resident.HouseNo ?? ""}-{resident.Level ?? ""}-{resident.BlockNo ?? ""}-{resident.RoadNo ?? ""}"

                    });
                }

                result.Add(new ResidentFaceProfile
                {
                    ResidentId = resident.Id,
                    MaintenanceBillStatus = MaintanancePaymentStatus?.PaymentStatus ?? "Pending",// placeholder
                    ResidentImage = images
                });
            }

            return result;
        }





        public async Task<bool> UpdateResidentFaceEntryExitAsync(ResidentFaceEntryExitRequest request)
        {
            ConvertRequestObjectToJson(request);
            try
            {
                //var record = new Resident
                //{
                //    CommunityId = request.CommunityId,
                //    FaceImageBase64 = request.FaceImageBase64,
                //    EntryTime = request.EntryTime,
                //    ExitTime = request.ExitTime
                //};
                //_context.ResidentFaceEntryExits.Add(record);
                //await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
               
                return false;
            }
        }

        // Visitor – QR Code
        public async Task<List<VisitorQrProfile>> GetVisitorQrListAsync(string communityId)
        {
            ConvertRequestObjectToJson(communityId);
            var comId = await _context.Community.Where(x => x.CommunityId == communityId).FirstOrDefaultAsync();
            if (comId == null)
                throw new Exception("No community found");
            return await _context.VisitorAccessDetails.Where(x => x.VisitorAccessType.Id == 2)
                .Where(q => q.CommunityId == comId.Id)
               .Select(q => new VisitorQrProfile
               {
                   VisitorId = q.Id,
                   VehiclePlateNo=q.VehicleNo,
                   ValidFrom = q.CreatedDate,
                   ValidTo = q.CreatedDate,
                   QrCodeString = $"Community Name: {comId.CommunityName} | Visitor Name: {q.VisitorName} | Vehicle No: {q.VehicleNo} | Visit Date: {(q.VisitDate.HasValue ? q.VisitDate.Value.ToString("dd-MM-yyyy") : "")}"

               })
.ToListAsync();
        }

        // Visitor – QR Code
        public async Task<List<VisitorPlateProfile>> GetVisitorListAsync(string communityId)
        {
            ConvertRequestObjectToJson(communityId);
            var comId = await _context.Community.Where(x => x.CommunityId == communityId).FirstOrDefaultAsync();
            if (comId == null)
                throw new Exception("No community found");
            return await _context.VisitorAccessDetails.Where(x=>x.VisitorAccessType.Id==1)
                .Where(q => q.CommunityId == comId.Id)
                .Select(q => new VisitorPlateProfile
                {
                    VisitorId = q.Id,
                    PlateNo =q.VehicleNo,
                    IsActive= true,
                    Entry = q.VisitDate.HasValue ? q.VisitDate.Value.ToString("dd/MM/yyyy") : null,
                    Exit = q.VisitDate.HasValue ? q.VisitDate.Value.ToString("dd/MM/yyyy") : null,
                    PaymentStatus = _context.VisitorPaymentStatus
                    .Where(x => x.VisitorId == q.Id)
                    .Select(x => x.PaymentStatus)
                    .FirstOrDefault() ?? "Pending"
                    //IsActive = q.Status
                }).ToListAsync();
        }

        public async Task<VisitorPlateProfile?> GetRegisteredVisitorAsync(string communityId, string vehiclePlateNo)
        {
            // Validate inputs early
            if (string.IsNullOrWhiteSpace(communityId))
                throw new ArgumentException("CommunityId is required.", nameof(communityId));
            if (string.IsNullOrWhiteSpace(vehiclePlateNo))
                throw new ArgumentException("VehiclePlateNo is required.", nameof(vehiclePlateNo));

            // Optional: logging/debug purpose
            ConvertRequestObjectToJson(communityId);

            // Fetch community Id directly (projecting only the needed column)
            var comId = await _context.Community
                .Where(c => c.CommunityId == communityId)
                .Select(c => c.Id)
                .FirstOrDefaultAsync();

            if (comId == 0)
                throw new Exception("No community found.");

            // Fetch visitor details
            var visitor = await _context.VisitorAccessDetails
                .Where(v => v.CommunityId == comId && v.VehicleNo == vehiclePlateNo)
                .OrderByDescending(v => v.CreatedDate) // Optional: get latest entry
                .Select(v => new VisitorPlateProfile
                {
                    VisitorId = v.Id,
                    PlateNo = v.VehicleNo,
                    IsActive = true, // or map from v.Status if exists
                    Entry = v.VisitDate.HasValue ? v.VisitDate.Value.ToString("dd/MM/yyyy") : null,
                    Exit = v.VisitDate.HasValue ? v.VisitDate.Value.ToString("dd/MM/yyyy") : null,
                    PaymentStatus = _context.VisitorPaymentStatus
                    .Where(x => x.VisitorId == v.Id)
                    .Select(x => x.PaymentStatus)
                    .FirstOrDefault() ?? "Pending"

                })
                .FirstOrDefaultAsync();

            return visitor;
        }


        public async Task<bool> CheckVisitorQrStatusAsync(string communityId, string qrCodeString)
        {
            return true;
        }

        public async Task<bool> UpdateVisitorQrEntryExitAsync(VisitorQrEntryExitRequest request)
        {
            ConvertRequestObjectToJson(request);
            try
            {
                ConvertRequestObjectToJson(request);
                var comId = await _context.Community.Where(x => x.CommunityId == request.CommunityId).FirstOrDefaultAsync();
                if (comId == null)
                    throw new Exception("No community found");
                var record = new VisitorAccessDetails
                {

                    CommunityId = comId.Id,
                    //QrCodeString = request.QrCodeString,
                    EntryTime = request.EntryTime,
                    ExitTime = request.ExitTime
                };
                _context.VisitorAccessDetails.Add(record);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
               // _logger.LogError(ex, "Error saving visitor QR entry/exit.");
                return false;
            }
        }


        private Boolean ConvertRequestObjectToJson(object req)
        {
            var rawreq= System.Text.Json.JsonSerializer.Serialize(req, new System.Text.Json.JsonSerializerOptions
            {
                WriteIndented = true
            });
            MicrobitRequestResponse request=new MicrobitRequestResponse();
            request.RequestJson = rawreq;
            request.RequestDate=DateTime.Now;
            request.CreatedDate = DateTime.Now;
            _context.MicrobitRequestResponses.Add(request); 
            _context.SaveChanges();
            return true;
        }

        public async Task<string> GetCommunityAllowAccess(string communityId)
        {
            var comId = await _context.Community.Where(x => x.CommunityId == communityId).FirstOrDefaultAsync();
            if (comId == null)
                throw new Exception("No community found");
            return comId?.AllowAccess;
        }


    }
}
