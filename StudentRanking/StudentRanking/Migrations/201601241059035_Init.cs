namespace StudentRanking.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Init : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.RankingDates",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FirstRankingDate = c.String(),
                        SecondRankingDate = c.String(),
                        ThirdRankingDate = c.String(),
                        PreferrencesFirstDate = c.String(),
                        PreferrencesLastDate = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ExamNames",
                c => new
                    {
                        Name = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Name);
            
            CreateTable(
                "dbo.Exams",
                c => new
                    {
                        ExamName = c.String(nullable: false, maxLength: 128),
                        StudentEGN = c.String(nullable: false, maxLength: 128),
                        Grade = c.Double(nullable: false),
                    })
                .PrimaryKey(t => new { t.ExamName, t.StudentEGN });
            
            CreateTable(
                "dbo.Faculties",
                c => new
                    {
                        ProgrammeName = c.String(nullable: false, maxLength: 128),
                        FacultyName = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.ProgrammeName);
            
            CreateTable(
                "dbo.FacultyRankLists",
                c => new
                    {
                        ProgrammeName = c.String(nullable: false, maxLength: 128),
                        EGN = c.String(nullable: false, maxLength: 128),
                        TotalGrade = c.Double(nullable: false),
                    })
                .PrimaryKey(t => new { t.ProgrammeName, t.EGN });
            
            CreateTable(
                "dbo.Formulae",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProgrammeName = c.String(nullable: false),
                        C1 = c.Double(nullable: false),
                        X = c.String(),
                        C2 = c.Double(nullable: false),
                        Y = c.String(),
                        C3 = c.Double(nullable: false),
                        Z = c.String(),
                        C4 = c.Double(nullable: false),
                        W = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Preferences",
                c => new
                    {
                        EGN = c.String(nullable: false, maxLength: 128),
                        PrefNumber = c.Int(nullable: false),
                        ProgrammeName = c.String(nullable: false),
                        TotalGrade = c.Double(nullable: false),
                    })
                .PrimaryKey(t => new { t.EGN, t.PrefNumber });
            
            CreateTable(
                "dbo.ProgrammeRules",
                c => new
                    {
                        ProgrammeName = c.String(nullable: false, maxLength: 128),
                        MaleCount = c.Int(nullable: false),
                        FemaleCount = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ProgrammeName);
            
            CreateTable(
                "dbo.Students",
                c => new
                    {
                        EGN = c.String(nullable: false, maxLength: 128),
                        FirstName = c.String(nullable: false),
                        LastName = c.String(nullable: false),
                        Email = c.String(nullable: false),
                        Gender = c.Boolean(nullable: false),
                        IsEnrolled = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.EGN);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Students");
            DropTable("dbo.ProgrammeRules");
            DropTable("dbo.Preferences");
            DropTable("dbo.Formulae");
            DropTable("dbo.FacultyRankLists");
            DropTable("dbo.Faculties");
            DropTable("dbo.Exams");
            DropTable("dbo.ExamNames");
            DropTable("dbo.RankingDates");
        }
    }
}
