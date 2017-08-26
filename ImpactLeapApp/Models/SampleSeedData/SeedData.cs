using ImpactLeapApp.Data;
using ImpactLeapApp.Models.OrderModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImpactLeapApp.Models.SampleSeedData
{
    public class SeedData
    {
        public static void Initialize(ApplicationDbContext db)
        {
            GetModules(db);
            GetPortfolios(db);
            GetSavings(db);
            GetPromotions(db);
        }

        public static void GetModules(ApplicationDbContext db)
        {
            if (!db.Modules.Any())
            {
                db.Modules.Add(new Module()
                {
                    ModuleName = "Overview and Financials",
                    Description = "Description Overview and Financials",
                    UnitPrice = 0,
                    ModifiedDate = DateTime.Today,
                });
                db.Modules.Add(new Module()
                {
                    ModuleName = "Operational blueprint and asset-level data",
                    Description = "Description Operational blueprint and asset-level data",
                    UnitPrice = 25,
                    ModifiedDate = DateTime.Today,
                });
                db.Modules.Add(new Module()
                {
                    ModuleName = "Social Impact metrics",
                    Description = "Description Social Impact metrics",
                    UnitPrice = 25,
                    ModifiedDate = DateTime.Today,
                });
                db.Modules.Add(new Module()
                {
                    ModuleName = "Environmental impact metrics",
                    Description = "Description Environmental impact metrics",
                    UnitPrice = 25,
                    ModifiedDate = DateTime.Today,
                });
                db.Modules.Add(new Module()
                {
                    ModuleName = "Governance and controversies",
                    Description = "Description Governance and controversies",
                    UnitPrice = 25,
                    ModifiedDate = DateTime.Today,
                });
                db.Modules.Add(new Module()
                {
                    ModuleName = "Upstream and downstream supplier analysis",
                    Description = "Description Upstream and downstream supplier analysis",
                    UnitPrice = 25,
                    ModifiedDate = DateTime.Today,
                });
                db.Modules.Add(new Module()
                {
                    ModuleName = "Regulatory, climate-realted and other risk analysis",
                    Description = "Description Regulatory, climate-realted and other risk analysis",
                    UnitPrice = 25,
                    ModifiedDate = DateTime.Today,
                });
                db.Modules.Add(new Module()
                {
                    ModuleName = "Benchmarking and targets",
                    Description = "Description Benchmarking and targets",
                    UnitPrice = 25,
                    ModifiedDate = DateTime.Today,
                });
                db.SaveChanges();
            }
        }

        public static void GetPortfolios(ApplicationDbContext db)
        {
            if (!db.Portfolios.Any())
            {
                db.Portfolios.Add(new Portfolio()
                {
                    FundName = "A Absolute Return",
                    FundManager = "A",
                    Strategy = "High Yield",
                    Geography = "Global",
                    Description = "Global",
                    ModifiedDate = DateTime.Today,
                });

                db.Portfolios.Add(new Portfolio()
                {
                    FundName = "B Absolute Return",
                    FundManager = "B",
                    Strategy = "High Yield",
                    Geography = "Global",
                    Description = "Global",
                    ModifiedDate = DateTime.Today,
                });

                db.Portfolios.Add(new Portfolio()
                {
                    FundName = "C Absolute Return",
                    FundManager = "C",
                    Strategy = "High Yield",
                    Geography = "Global",
                    Description = "Global",
                    ModifiedDate = DateTime.Today,
                });

                db.Portfolios.Add(new Portfolio()
                {
                    FundName = "D Absolute Return",
                    FundManager = "D",
                    Strategy = "High Yield",
                    Geography = "Global",
                    Description = "Global",
                    ModifiedDate = DateTime.Today,
                });

                db.Portfolios.Add(new Portfolio()
                {
                    FundName = "E Absolute Return",
                    FundManager = "E",
                    Strategy = "High Yield",
                    Geography = "Global",
                    Description = "Global",
                    ModifiedDate = DateTime.Today,
                });

                db.Portfolios.Add(new Portfolio()
                {
                    FundName = "F Absolute Return",
                    FundManager = "F",
                    Strategy = "High Yield",
                    Geography = "Global",
                    Description = "Global",
                    ModifiedDate = DateTime.Today,
                });

                db.Portfolios.Add(new Portfolio()
                {
                    FundName = "G Absolute Return",
                    FundManager = "G",
                    Strategy = "High Yield",
                    Geography = "Global",
                    Description = "Global",
                    ModifiedDate = DateTime.Today,
                });

                db.Portfolios.Add(new Portfolio()
                {
                    FundName = "H Absolute Return",
                    FundManager = "H",
                    Strategy = "High Yield",
                    Geography = "Global",
                    Description = "Global",
                    ModifiedDate = DateTime.Today,
                });

                db.Portfolios.Add(new Portfolio()
                {
                    FundName = "I Absolute Return",
                    FundManager = "I",
                    Strategy = "High Yield",
                    Geography = "Global",
                    Description = "Global",
                    ModifiedDate = DateTime.Today,
                });

                db.Portfolios.Add(new Portfolio()
                {
                    FundName = "J Absolute Return",
                    FundManager = "J",
                    Strategy = "High Yield",
                    Geography = "Global",
                    Description = "Global",
                    ModifiedDate = DateTime.Today,
                });

                db.Portfolios.Add(new Portfolio()
                {
                    FundName = "K Absolute Return",
                    FundManager = "K",
                    Strategy = "High Yield",
                    Geography = "Global",
                    Description = "Global",
                    ModifiedDate = DateTime.Today,
                });
                db.SaveChanges();
            }

        }

        public static void GetSavings(ApplicationDbContext db)
        {
            if (!db.Savings.Any())
            {
                db.Savings.Add(new Saving()
                {
                    SavingName = "Discount Fixed",
                    DiscountMethod = SavingDiscountMethodList.Fixed,
                    SavingRate = 10,
                    SelectFrom = 2,
                    SelectTo = 4,
                    Description = "Discount $10 for selections of 2 to 4",
                    ModifiedDate = DateTime.Today,
                });

                db.Savings.Add(new Saving()
                {
                    SavingName = "Discount Percent",
                    DiscountMethod = SavingDiscountMethodList.Percentage,
                    SavingRate = 15,
                    SelectFrom = 5,
                    SelectTo = 8,
                    Description = "Discount 15% for selections of 5 to 8",
                    ModifiedDate = DateTime.Today,
                });

                db.SaveChanges();
            }
        }

        public static void GetPromotions(ApplicationDbContext db)
        {
            if (!db.Promotions.Any())
            {
                db.Promotions.Add(new Promotion()
                {
                    PromotionName = "Promotion Sample - Fixed",
                    PromotionCode = "AAAAAAAA",
                    DiscountMethod = PromotionDiscountMethodList.Fixed,
                    DiscountRate = 10,
                    DateFrom = DateTime.Today,
                    DateTo = DateTime.Today.AddYears(1),
                    IsActive = true,
                    Description = "Sample Promotion - fixed discount rate - 10",
                    ModifiedDate = DateTime.Today,
                });

                db.Promotions.Add(new Promotion()
                {
                    PromotionName = "Promotion Sample - Percentage",
                    PromotionCode = "BBBBBBBB",
                    DiscountMethod = PromotionDiscountMethodList.Percentage,
                    DiscountRate = 10,
                    DateFrom = DateTime.Today,
                    DateTo = DateTime.Today.AddYears(1),
                    IsActive = true,
                    Description = "Sample Promotion - percentage discount rate - 10",
                    ModifiedDate = DateTime.Today,
                });
            }
        }
    }
}
