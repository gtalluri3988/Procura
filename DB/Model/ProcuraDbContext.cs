
using DB.EFModel;
using DB.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB.Model
{
    public class ProcuraDbContext : DbContext
    {

        public ProcuraDbContext(DbContextOptions<ProcuraDbContext> options)
            : base(options)
        {
        }


        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<RoleMenuPermission> RoleMenuPermissions { get; set; }
        public DbSet<CodeSystem> CodeSystems { get; set; }
        public DbSet<CodeHierarchy> CodeHierarchies { get; set; }
        public DbSet<Menu> Menus { get; set; }
        public DbSet<LoginHistory> LoginHistories { get; set; }

        public DbSet<UserPasswordHistory> UserPasswordHistories { get; set; }

        public DbSet<PasswordPolicy> passwordPolicies { get; set; }

        public DbSet<Notification> Notifications { get; set; }

        public DbSet<PaymentRequest> PaymentRequest { get; set; }

        public DbSet<WebHookResponse> WebHookResponses { get; set; }
        public DbSet<PaymentTransactionResponse> PaymentTransactionResponses { get; set; }

        public DbSet<ContentManagement> ContentManagement { get; set; }

        public DbSet<ContentPicture> ContentPictures { get; set; }

       
        public DbSet<TenderDocument> TenderDocuments { get; set; }
        public DbSet<TenderEvaluationCriteria> TenderEvaluationCriteria { get; set; }
        public DbSet<JobCategory> JobCategories { get; set; }
        public DbSet<InternalOrder> InternalOrders { get; set; }
        public DbSet<JobScope> JobScopes { get; set; }
        public DbSet<VendorManagementSetting> VendorManagementSettings { get; set; }
        public DbSet<CategorySettings> CategorySettings { get; set; }

        public DbSet<Vendor> Vendors { get; set; }
        public DbSet<VendorMember> VendorMembers { get; set; }
        public DbSet<VendorManpower> VendorManpowers { get; set; }
        public DbSet<VendorFinancial> VendorFinancials { get; set; }
        public DbSet<VendorCreditFacility> VendorCreditFacilities { get; set; }
        public DbSet<VendorTax> VendorTaxes { get; set; }
        public DbSet<VendorBank> VendorBanks { get; set; }
        public DbSet<VendorCategory> VendorCategories { get; set; }
        public DbSet<VendorExperience> VendorExperiences { get; set; }
        public DbSet<VendorDeclaration> VendorDeclarations { get; set; }

        public DbSet<VendorShareholder> VendorShareholder { get; set; }
        public DbSet<VendorManpower> VendorManpower { get; set; }
        public DbSet<VendorStaffDeclaration> VendorStaffDeclaration { get; set; }

        public DbSet<VendorDirector> VendorDirectors { get; set; }
        

        public DbSet<SSMResponse> SSMResponses { get; set; }

        public DbSet<State> State { get; set; }

        public DbSet<SiteLevel> SiteLevel { get; set; }

        public DbSet<CompanyCategory> companyCategories { get; set; }
        public DbSet<CompanyEntityType> CompanyEntityTypes { get; set; }

        public DbSet<VendorPaymentStatus> VendorPaymentStatus { get; set; }

        public DbSet<IndustryType> IndustryTypes { get; set; }

        public DbSet<Country> Countries { get; set; }

        public DbSet<PaymentMethod> PaymentMethods { get; set; }

        public DbSet<Vendor_SAPRequestResponse> Vendor_SAPRequestResponses { get; set; }

        public DbSet<PaymentChannel> PaymentChannel { get; set; }

        public DbSet<TenderJobScope> TenderJobScopes { get; set; }
        public DbSet<TenderRequiredDocument> TenderRequiredDocuments { get; set; }
        public DbSet<TenderReview> TenderReviews { get; set; }
        public DbSet<TenderApproval> TenderApprovals { get; set; }

        public DbSet<TenderApplication> TenderApplications { get; set; }


        public DbSet<CodeMaster> CodeMasters { get; set; }

        public DbSet<Category> Categories { get; set; }
        public DbSet<SubCategory> SubCategories { get; set; }
        public DbSet<Activity> Activities { get; set; }
        public DbSet<TenderSetting> TenderSettings { get; set; }
        public DbSet<MaterialBudget> MaterialBudgets { get; set; }
        public DbSet<MaterialBudgetUpload> MaterialBudgetUploads { get; set; }

        public DbSet<CategoryCodeSetting> CategoryCodeSetting { get; set; }

        public DbSet<VendorCategoryCertificate> VendorCategoryCertificates { get; set; }

        public DbSet<TenderCategoryCode> TenderCategoryCodes { get; set; }
        public DbSet<TenderSiteVisit> TenderSiteVisits { get; set; }

        public DbSet<TenderCategory> TenderCategories { get; set; }

        public DbSet<TenderApplicationStatus> TenderApplicationStatus { get; set; }

     

        public DbSet<Announcement> Announcements { get; set; }

        public DbSet<Questionnaire> Questionnaires { get; set; }

        public DbSet<Question> Questions { get; set; }

        public DbSet<Designation> Designations { get; set; }

        public DbSet<QuestionAnswer> QuestionAnswers { get; set; }

        public DbSet<TenderAdvertisementSetting> TenderAdvertisementSettings { get; set; }

        public DbSet<TenderOpeningCommittee> TenderOpeningCommittee { get; set; }

        public DbSet<TenderEvaluationCommittee> TenderEvaluationCommittee { get; set; }

        public DbSet<EvaluationCriteria> EvaluationCriteria { get; set; }
        public DbSet<TenderEvaluationSpecification> TenderEvaluationSpecifications { get; set; }
        public DbSet<TenderIssuenceApproval> TenderIssuenceApprovals { get; set; }

        public DbSet<TenderVendorSubmission> TenderVendorSubmissions { get; set; }
        public DbSet<TenderTechnicalEvaluationScore> TenderTechnicalEvaluationScores { get; set; }
        public DbSet<TenderTechnicalEvaluationResult> TenderTechnicalEvaluationResults { get; set; }
        public DbSet<TenderRecommendation> TenderRecommendations { get; set; }

        // Tender Award
        public DbSet<TenderAward> TenderAwards { get; set; }
        public DbSet<TenderAwardMinutesOfMeeting> TenderAwardMinutesOfMeetings { get; set; }

        // Vendor Performance
        public DbSet<VendorPerformance> VendorPerformances { get; set; }
        public DbSet<VendorPerformanceScore> VendorPerformanceScores { get; set; }
        public DbSet<VendorPerformanceFeedback> VendorPerformanceFeedbacks { get; set; }

        // Bidding
        public DbSet<BiddingAsset> BiddingAssets { get; set; }
        public DbSet<BidderSubmissionItem> BidderSubmissionItems { get; set; }
        public DbSet<TenderOpeningVerification> TenderOpeningVerifications { get; set; }
        public DbSet<BidderAcknowledgement> BidderAcknowledgements { get; set; }

        // Category Code Change Audit
        public DbSet<VendorCategoryChangeLog> VendorCategoryChangeLogs { get; set; }

        // Category Code Approval Workflow
        public DbSet<CategoryCodeApproval> CategoryCodeApprovals { get; set; }
        public DbSet<CategoryCodeApprovalItem> CategoryCodeApprovalItems { get; set; }

        // Bank Key Master
        public DbSet<BankKey> BankKeys { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Vendor>()
            .Property(v => v.CurrentStep)
            .HasConversion<string>();

       //     modelBuilder.Entity<Vendor>()
       //.HasMany(v => v.Shareholders)
       //.WithOne(s => s.Vendor)
       //.HasForeignKey(s => s.VendorId)
       //.OnDelete(DeleteBehavior.Cascade);

       //     modelBuilder.Entity<Vendor>()
       //         .HasMany(v => v.Directors)
       //         .WithOne(d => d.Vendor)
       //         .HasForeignKey(d => d.VendorId)
       //         .OnDelete(DeleteBehavior.Cascade);

       //     modelBuilder.Entity<Vendor>()
       //         .HasMany(v => v.StaffDeclarations)
       //         .WithOne(sd => sd.Vendor)
       //         .HasForeignKey(sd => sd.VendorId)
       //         .OnDelete(DeleteBehavior.Cascade);

    //        modelBuilder.Entity<CodeHierarchy>()
    //.HasOne(m => m.Parent)
    //.WithMany(m => m.Children)
    //.HasForeignKey(m => m.ParentId);

            modelBuilder.Entity<CodeHierarchy>()
        .HasOne(c => c.Parent)
        .WithMany(c => c.Children)
        .HasForeignKey(c => c.ParentId)
        .OnDelete(DeleteBehavior.Restrict); // ✅ important



            //modelBuilder.Entity<VendorCategory>()
            //    .HasOne(vc => vc.MasterCategory)
            //    .WithMany(m => m.VendorCategories)
            //    .HasForeignKey(vc => vc.MasterCategoryId);

            //modelBuilder.Entity<VendorCategory>()
            //    .HasOne(vc => vc.Vendor)
            //    .WithMany(v => v.VendorCategories)
            //    .HasForeignKey(vc => vc.VendorId);


            // Disable OUTPUT clause if triggers exist (SQL Server)
            //foreach (var entity in modelBuilder.Model.GetEntityTypes())
            //{
            //    entity.SetUseSqlOutputClause(false);
            //}


            //        modelBuilder.Entity<TenderApproval>()
            //.HasOne(x => x.ApprovedByUser)
            //.WithMany()
            //.HasForeignKey(x => x.ApprovedByUserId)
            //.OnDelete(DeleteBehavior.NoAction);

            //        modelBuilder.Entity<TenderReview>()
            //            .HasOne(x => x.ReviewedByUser)
            //            .WithMany()
            //            .HasForeignKey(x => x.ReviewedByUserId)
            //            .OnDelete(DeleteBehavior.NoAction);

            // Category Code Approval — relationships
            modelBuilder.Entity<CategoryCodeApproval>()
                .HasOne(a => a.Vendor)
                .WithMany()
                .HasForeignKey(a => a.VendorId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CategoryCodeApproval>()
                .HasOne(a => a.Reviewer)
                .WithMany()
                .HasForeignKey(a => a.ReviewedBy)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<CategoryCodeApprovalItem>()
                .HasOne(i => i.CategoryCodeApproval)
                .WithMany(a => a.Items)
                .HasForeignKey(i => i.CategoryCodeApprovalId)
                .OnDelete(DeleteBehavior.Cascade);

        }
    }

}
