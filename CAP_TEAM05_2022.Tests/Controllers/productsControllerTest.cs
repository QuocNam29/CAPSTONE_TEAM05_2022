using CAP_TEAM05_2022.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CAP_TEAM05_2022.Controllers.Tests
{
    [TestClass]
    public class productsControllerTest
    {
        public class MockHttpSession : HttpSessionStateBase
        {
            public Hashtable buffer = new Hashtable();
            public override object this[string key]
            {
                get
                {
                    return buffer[key];
                }
                set
                {
                    buffer[key] = value;
                }
            }
        }
        private IList<ValidationResult> ValidateModel(object model)
        {
            var validationResults = new List<ValidationResult>();
            var ctx = new ValidationContext(model, null, null);
            Validator.TryValidateObject(model, ctx, validationResults, true);
            return validationResults;
        }

        [TestMethod()]
        public void Check_False_If_Name_Product_Null_Test()
        {
            // Arrange
            var controller = new productsController();
            var db = new CP25Team05Entities();

            // Act          
            var model = new product()
            {
                name = null
            };

            // Assert
            Assert.IsTrue(ValidateModel(model).Where(x => x.ErrorMessage.Equals("Vui lòng nhập tên sản phẩm !")).Count() > 0);
        }

        [TestMethod()]
        public void Check_False_If_Name_Product_MoreThan_100_Characters_Test()
        {
            // Arrange
            var controller = new productsController();
            var db = new CP25Team05Entities();

            // Act          
            var model = new product()
            {
                name = "11111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111"
            };

            // Assert
            Assert.IsTrue(ValidateModel(model).Where(x => x.ErrorMessage.Equals("Tên sản phẩm phải dưới 255 ký tự.")).Count() > 0);
        }

        [TestMethod()]
        public void Check_False_If_Code_Null_Test()
        {
            // Arrange
            var controller = new productsController();
            var db = new CP25Team05Entities();

            // Act          
            var model = new product()
            {
                code = null
            };

            // Assert
            Assert.IsTrue(ValidateModel(model).Where(x => x.ErrorMessage.Equals("Vui lòng nhập mã sản phẩm")).Count() > 0);
        }

        [TestMethod()]
        public void Check_False_If_Code_MoreThan_100_Characters_Test()
        {
            // Arrange
            var controller = new productsController();
            var db = new CP25Team05Entities();

            // Act          
            var model = new product()
            {
                code = "11111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111"
            };

            // Assert
            Assert.IsTrue(ValidateModel(model).Where(x => x.ErrorMessage.Equals("Mã sản phẩm phải dưới 100 ký tự.")).Count() > 0);
        }

        [TestMethod()]
        public void Check_False_If_Unit_Null_Test()
        {
            // Arrange
            var controller = new productsController();
            var db = new CP25Team05Entities();

            // Act          
            var model = new product()
            {
                unit = null
            };

            // Assert
            Assert.IsTrue(ValidateModel(model).Where(x => x.ErrorMessage.Equals("Vui lòng nhập tên đơn vị tính !")).Count() > 0);
        }

        [TestMethod()]
        public void Check_False_If_Unit_MoreThan_100_Characters_Test()
        {
            // Arrange
            var controller = new productsController();
            var db = new CP25Team05Entities();

            // Act          
            var model = new product()
            {
                unit = "11111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111"
            };

            // Assert
            Assert.IsTrue(ValidateModel(model).Where(x => x.ErrorMessage.Equals("Đơn vị sản phẩm phải dưới 100 ký tự.")).Count() > 0);
        }

        [TestMethod()]
        public void Check_False_If_purchase_price_Value_bigger_than_0_Test()
        {
            // Arrange
            var controller = new productsController();
            var db = new CP25Team05Entities();

            // Act          
            var model = new product()
            {
                purchase_price = -2
            };

            // Assert
            Assert.IsTrue(ValidateModel(model).Where(x => x.ErrorMessage.Count() >0).Count() > 0);
        }

        [TestMethod()]
        public void Check_False_If_sell_price_Value_bigger_than_0_Test()
        {
            // Arrange
            var controller = new productsController();
            var db = new CP25Team05Entities();

            // Act          
            var model = new product()
            {
                sell_price = -2
            };

            // Assert
            Assert.IsTrue(ValidateModel(model).Where(x => x.ErrorMessage.Count() > 0).Count() > 0);
        }

        [TestMethod()]
        public void Check_False_If_sell_price_debt_Value_bigger_than_0_Test()
        {
            // Arrange
            var controller = new productsController();
            var db = new CP25Team05Entities();

            // Act          
            var model = new product()
            {
                sell_price_debt = -2
            };

            // Assert
           
        }



        [TestMethod()]
        public void Check_False_If_quantity_Value_bigger_than_0_Test()
        {
            // Arrange
            var controller = new productsController();
            var db = new CP25Team05Entities();

            // Act          
            var model = new product()
            {
                quantity = -2
            };

            // Assert
            Assert.IsTrue(ValidateModel(model).Where(x => x.ErrorMessage.Count() >0 ).Count() > 0);
        }

        [TestMethod()]
        public void Check_False_If_Unit_swap_Null_Test()
        {
            // Arrange
            var controller = new productsController();
            var db = new CP25Team05Entities();

            // Act          
            var model = new product()
            {
                unit_swap = null
            };

            // Assert
            Assert.IsTrue(ValidateModel(model).Where(x => x.ErrorMessage.Equals("Vui lòng nhập đơn vị quy đổi")).Count() > 0);
        }

        [TestMethod()]
        public void Check_False_If_Unit_swap_MoreThan_100_Characters_Test()
        {
            // Arrange
            var controller = new productsController();
            var db = new CP25Team05Entities();

            // Act          
            var model = new product()
            {
                unit_swap = "11111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111"
            };

            // Assert
            Assert.IsTrue(ValidateModel(model).Where(x => x.ErrorMessage.Equals("Đơn vị quy đổi phải dưới 100 ký tự.")).Count() > 0);
        }

        [TestMethod()]
        public void Check_False_If_Quantity_swap_Value_bigger_than_0_Test()
        {
            // Arrange
            var controller = new productsController();
            var db = new CP25Team05Entities();

            // Act          
            var model = new product()
            {
                quantity_swap = -2
            };

            // Assert
            Assert.IsTrue(ValidateModel(model).Where(x => x.ErrorMessage.Count() >0 ).Count() > 0);
        }

        [TestMethod()]
        public void Check_False_If_sell_price_swap_Value_bigger_than_0_Test()
        {
            // Arrange
            var controller = new productsController();
            var db = new CP25Team05Entities();

            // Act          
            var model = new product()
            {
                sell_price_swap = -2
            };

            // Assert
            Assert.IsTrue(ValidateModel(model).Where(x => x.ErrorMessage.Count() > 0).Count() > 0);
        }

        [TestMethod()]
        public void Check_False_If_sell_price_debt_swap_Value_bigger_than_0_Test()
        {
            // Arrange
            var controller = new productsController();
            var db = new CP25Team05Entities();

            // Act          
            var model = new product()
            {
                sell_price_debt_swap = -2
            };

            // Assert
            Assert.IsTrue(ValidateModel(model).Where(x => x.ErrorMessage.Count() > 0).Count() > 0);
        }
    }
}
