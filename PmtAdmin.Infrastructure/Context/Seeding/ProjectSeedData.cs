using Microsoft.EntityFrameworkCore;
using PmtAdmin.Domain.Entities;
using System;
using System.Collections.Generic;

namespace PmtAdmin.Infrastructure.Context.Seeding
{
    public static class ProjectSeedData
    {
        public static List<Project> GetProjects()
        {
            var projects = new List<Project>();
            var random = new Random(42); // Fixed seed for consistency

            // Pre-generated GUIDs (hardcoded)
            var projectGuids = new[]
            {
                new Guid("10000000-0000-0000-0000-000000000001"),
                new Guid("10000000-0000-0000-0000-000000000002"),
                new Guid("10000000-0000-0000-0000-000000000003"),
                new Guid("10000000-0000-0000-0000-000000000004"),
                new Guid("10000000-0000-0000-0000-000000000005"),
                new Guid("10000000-0000-0000-0000-000000000006"),
                new Guid("10000000-0000-0000-0000-000000000007"),
                new Guid("10000000-0000-0000-0000-000000000008"),
                new Guid("10000000-0000-0000-0000-000000000009"),
                new Guid("10000000-0000-0000-0000-000000000010"),
                new Guid("10000000-0000-0000-0000-000000000011"),
                new Guid("10000000-0000-0000-0000-000000000012"),
                new Guid("10000000-0000-0000-0000-000000000013"),
                new Guid("10000000-0000-0000-0000-000000000014"),
                new Guid("10000000-0000-0000-0000-000000000015"),
                new Guid("10000000-0000-0000-0000-000000000016"),
                new Guid("10000000-0000-0000-0000-000000000017"),
                new Guid("10000000-0000-0000-0000-000000000018"),
                new Guid("10000000-0000-0000-0000-000000000019"),
                new Guid("10000000-0000-0000-0000-000000000020"),
                new Guid("10000000-0000-0000-0000-000000000021"),
                new Guid("10000000-0000-0000-0000-000000000022"),
                new Guid("10000000-0000-0000-0000-000000000023"),
                new Guid("10000000-0000-0000-0000-000000000024"),
                new Guid("10000000-0000-0000-0000-000000000025"),
                new Guid("10000000-0000-0000-0000-000000000026"),
                new Guid("10000000-0000-0000-0000-000000000027"),
                new Guid("10000000-0000-0000-0000-000000000028"),
                new Guid("10000000-0000-0000-0000-000000000029"),
                new Guid("10000000-0000-0000-0000-000000000030"),
                new Guid("10000000-0000-0000-0000-000000000031"),
                new Guid("10000000-0000-0000-0000-000000000032"),
                new Guid("10000000-0000-0000-0000-000000000033"),
                new Guid("10000000-0000-0000-0000-000000000034"),
                new Guid("10000000-0000-0000-0000-000000000035"),
                new Guid("10000000-0000-0000-0000-000000000036"),
                new Guid("10000000-0000-0000-0000-000000000037"),
                new Guid("10000000-0000-0000-0000-000000000038"),
                new Guid("10000000-0000-0000-0000-000000000039"),
                new Guid("10000000-0000-0000-0000-000000000040"),
                new Guid("10000000-0000-0000-0000-000000000041"),
                new Guid("10000000-0000-0000-0000-000000000042"),
                new Guid("10000000-0000-0000-0000-000000000043"),
                new Guid("10000000-0000-0000-0000-000000000044"),
                new Guid("10000000-0000-0000-0000-000000000045"),
                new Guid("10000000-0000-0000-0000-000000000046"),
                new Guid("10000000-0000-0000-0000-000000000047"),
                new Guid("10000000-0000-0000-0000-000000000048"),
                new Guid("10000000-0000-0000-0000-000000000049"),
                new Guid("10000000-0000-0000-0000-000000000050")
            };

            // Base date for created_at (2 years ago from a fixed date)
            var baseDate = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            var projectNames = new[]
            {
                "Customer Portal Redesign", "Mobile Banking App", "ERP System Implementation",
                "Cloud Migration Initiative", "Analytics Dashboard", "Supply Chain Optimization",
                "CRM Integration", "Payment Gateway Development", "Inventory Management System",
                "HR Management Portal", "E-Commerce Platform", "Healthcare Management System",
                "Real Estate Portal", "Learning Management System", "Travel Booking Platform",
                "Food Delivery App", "Fitness Tracking Application", "Social Media Platform",
                "Video Streaming Service", "IoT Device Management", "Blockchain Implementation",
                "AI Chatbot Development", "Document Management System", "Fleet Management Solution",
                "Warehouse Automation", "Customer Support Portal", "Marketing Automation Platform",
                "Project Management Tool", "Time Tracking System", "Expense Management Application",
                "Recruitment Platform", "Performance Management System", "Asset Management Portal",
                "Compliance Tracking System", "Vendor Management Platform", "Contract Management System",
                "Risk Assessment Tool", "Quality Management System", "Audit Management Platform",
                "Billing and Invoicing System", "Subscription Management Portal", "Loyalty Program Platform",
                "Feedback Management System", "Survey Application", "Event Management Platform",
                "Booking Management System", "Appointment Scheduling App", "Notification Service",
                "Data Integration Platform", "API Gateway Development"
            };

            var customers = new[]
            {
                ("TechCorp Industries", "techcorp.com", "Global technology solutions provider"),
                ("FinServe Solutions", "finserve.io", "Financial services and banking"),
                ("HealthPlus Medical", "healthplus.org", "Healthcare and medical services"),
                ("RetailMax Group", "retailmax.com", "Retail and e-commerce solutions"),
                ("EduTech Systems", "edutech.edu", "Education technology platform"),
                ("LogiChain Corp", "logichain.com", "Logistics and supply chain management"),
                ("AutoDrive Inc", "autodrive.com", "Automotive and mobility solutions"),
                ("GreenEnergy Solutions", "greenenergy.com", "Renewable energy systems"),
                ("CloudFirst Technologies", "cloudfirst.io", "Cloud infrastructure services"),
                ("DataInsights Analytics", "datainsights.com", "Business intelligence and analytics"),
                ("SecureNet Systems", "securenet.com", "Cybersecurity solutions"),
                ("MediaStream Entertainment", "mediastream.tv", "Digital media and entertainment"),
                ("PropertyPro Realty", "propertypro.com", "Real estate management"),
                ("FoodHub Delivery", "foodhub.com", "Food delivery and restaurant tech"),
                ("TravelEase Booking", "travelease.com", "Travel and hospitality services"),
                ("FitLife Wellness", "fitlife.com", "Health and fitness technology"),
                ("SocialConnect Platform", "socialconnect.com", "Social networking services"),
                ("PaymentPro Gateway", "paymentpro.com", "Payment processing solutions"),
                ("InventoryMax Systems", "inventorymax.com", "Inventory and warehouse management"),
                ("HRElite Solutions", "hrelite.com", "Human resources technology")
            };

            var descriptions = new[]
            {
                "Strategic initiative to modernize and enhance digital capabilities",
                "Enterprise-level solution for improved operational efficiency",
                "Mission-critical system upgrade and transformation project",
                "Innovative platform development with cutting-edge technology",
                "Comprehensive integration and automation initiative",
                "Large-scale digital transformation program",
                "Customer-centric solution for enhanced user experience",
                "Data-driven platform for business intelligence",
                "Scalable cloud-native application development",
                "End-to-end process optimization and digitization"
            };

            var pocEmails = new[]
            {
                "john.smith@", "sarah.johnson@", "michael.brown@", "emily.davis@",
                "david.wilson@", "jennifer.taylor@", "robert.anderson@", "lisa.thomas@",
                "william.jackson@", "mary.white@", "james.harris@", "patricia.martin@",
                "richard.thompson@", "linda.garcia@", "charles.martinez@", "barbara.robinson@",
                "joseph.clark@", "susan.rodriguez@", "thomas.lewis@", "karen.lee@"
            };

            var pocPhones = new[]
            {
                "+1-555-0101", "+1-555-0102", "+1-555-0103", "+1-555-0104",
                "+1-555-0105", "+1-555-0106", "+1-555-0107", "+1-555-0108",
                "+1-555-0109", "+1-555-0110", "+44-20-7123-4567", "+44-20-7123-4568",
                "+91-22-1234-5678", "+91-22-1234-5679", "+61-2-1234-5678", "+61-2-1234-5679",
                "+49-30-1234-5678", "+49-30-1234-5679", "+33-1-1234-5678", "+33-1-1234-5679"
            };

            // Create 50 projects
            for (int i = 0; i < 50; i++)
            {
                var customerIndex = i % customers.Length;
                var customer = customers[customerIndex];
                var statusId = (i % 3) + 1; // Rotate between 1, 2, 3 (Active, Inactive, Complete)
                var deliveryUnitId = (i % 8) + 1; // Rotate between 1-8
                var projectManagerId = (i % 10) + 1; // Assuming 10 managers exist
                var pocIndex = i % pocEmails.Length;

                var project = new Project
                {
                    Id = projectGuids[i],
                    Name = projectNames[i],
                    Key = $"PROJ{(i + 1):D3}", // PROJ001, PROJ002, etc.
                    Description = descriptions[i % descriptions.Length],

                    // Customer Information
                    CustomerOrgName = customer.Item1,
                    CustomerDomainUrl = $"https://www.{customer.Item2}",
                    CustomerDescription = customer.Item3,
                    PocEmail = $"{pocEmails[pocIndex]}{customer.Item2}",
                    PocPhone = pocPhones[pocIndex],

                    // Management & Status
                    ProjectManagerId = projectManagerId,
                    ProjectManagerRoleId = random.Next(1, 4), // Assuming 3 PM roles exist
                    StatusId = statusId,
                    DeliveryUnitId = deliveryUnitId,
                    TemplateId = random.Next(1, 5), // Assuming 4 templates exist

                    // Audit fields - using fixed dates based on index
                    CreatedBy = 1,
                    UpdatedBy = i % 2 == 0 ? 1 : (int?)null,
                    CreatedAt = baseDate.AddDays(i * 5), // Stagger creation dates
                    UpdatedAt = i % 2 == 0 ? baseDate.AddDays((i * 5) + 30) : (DateTime?)null,

                    // Additional fields
                    IsImportedFromJira = i % 5 == 0, // 20% imported from Jira
                    Metadata = GenerateMetadata(i, statusId),
                    DeletedAt = null
                };

                projects.Add(project);
            }

            return projects;
        }

        private static string GenerateMetadata(int index, int statusId)
        {
            var priorities = new[] { "High", "Medium", "Low", "Critical" };
            var categories = new[] { "Web", "Mobile", "Desktop", "API", "Infrastructure" };
            var technologies = new[] { ".NET", "React", "Angular", "Node.js", "Python", "Java" };

            return $@"{{
                ""priority"": ""{priorities[index % priorities.Length]}"",
                ""category"": ""{categories[index % categories.Length]}"",
                ""technology"": ""{technologies[index % technologies.Length]}"",
                ""budget"": {50000 + (index * 10000)},
                ""estimatedHours"": {500 + (index * 100)},
                ""teamSize"": {5 + (index % 15)},
                ""isPublic"": {(index % 2 == 0).ToString().ToLower()},
                ""tags"": [""{categories[index % categories.Length]}"", ""{priorities[index % priorities.Length]}""],
                ""riskLevel"": ""{(index % 4 == 0 ? "High" : index % 3 == 0 ? "Medium" : "Low")}"",
                ""completionPercentage"": {(statusId == 3 ? 100 : statusId == 1 ? index % 80 : 0)}
            }}";
        }

        // Extension method to apply seed data
        public static void SeedProjects(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Project>().HasData(GetProjects());
        }
    }
}