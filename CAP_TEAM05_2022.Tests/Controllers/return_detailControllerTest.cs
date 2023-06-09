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
    public class return_detailControllerTest
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
        public void Check_False_If_Quantity_current_Value_bigger_than_0_Test()
        {
            // Arrange
            var controller = new return_detailControllerTest();
            var db = new CP25Team05Entities();

            // Act          
            var model = new return_details()
            {
                quantity_current = -2
            };

            // Assert
            Assert.IsTrue(ValidateModel(model).Where(x => x.ErrorMessage.Count() > 0).Count() > 0);
        }

        [TestMethod()]
        public void Check_False_If__current_Value_bigger_than_0_Test()
        {
            // Arrange
            var controller = new return_detailControllerTest();
            var db = new CP25Team05Entities();

            // Act          
            var model = new return_details()
            {
                total_current = -2
            };

            // Assert
            Assert.IsTrue(ValidateModel(model).Where(x => x.ErrorMessage.Equals("Vui lòng nhập giá trị phải lớn hơn 0")).Count() > 0);
        }

        [TestMethod()]
        public void Check_False_If__product_current_id_Value_bigger_than_0_Test()
        {
            // Arrange
            var controller = new return_detailControllerTest();
            var db = new CP25Team05Entities();

            // Act          
            var model = new return_details()
            {
                product_current_id = -2
            };

            // Assert
            Assert.IsTrue(ValidateModel(model).Where(x => x.ErrorMessage.Count() > 0).Count() > 0);
        }

        [TestMethod()]
        public void Check_False_If__return_id_Value_bigger_than_0_Test()
        {
            // Arrange
            var controller = new return_detailControllerTest();
            var db = new CP25Team05Entities();

            // Act          
            var model = new return_details()
            {
                return_id = -2
            };

            // Assert
            Assert.IsTrue(ValidateModel(model).Where(x => x.ErrorMessage.Count() > 0).Count() > 0);
        }
        [TestMethod()]
        public void Check_False_If_Unit_current_Null_Test()
        {
            // Arrange
            var controller = new return_detailControllerTest();
            var db = new CP25Team05Entities();

            // Act          
            var model = new return_details()
            {
                unit_current = null
            };

            // Assert
            Assert.IsTrue(ValidateModel(model).Where(x => x.ErrorMessage.Equals("Vui lòng nhập đơn vị tính !")).Count() > 0);
        }

        [TestMethod()]
        public void Check_False_If_Unit_current_MoreThan_100_Characters_Test()
        {
            // Arrange
            var controller = new return_detailControllerTest();
            var db = new CP25Team05Entities();

            // Act          
            var model = new return_details()
            {
                unit_current = "11111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111"
            };

            // Assert
            Assert.IsTrue(ValidateModel(model).Where(x => x.ErrorMessage.Equals("Đơn vị phải dưới 100 ký tự.")).Count() > 0);
        }

        [TestMethod()]
        public void Check_False_If_Unit_return_Null_Test()
        {
            // Arrange
            var controller = new return_detailControllerTest();
            var db = new CP25Team05Entities();

            // Act          
            var model = new return_details()
            {
                unit_return = null
            };

            // Assert
            Assert.IsTrue(ValidateModel(model).Where(x => x.ErrorMessage.Equals("Vui lòng nhập đơn vị trả !")).Count() > 0);
        }

        [TestMethod()]
        public void Check_False_If_Unit_return_MoreThan_100_Characters_Test()
        {
            // Arrange
            var controller = new return_detailControllerTest();
            var db = new CP25Team05Entities();

            // Act          
            var model = new return_details()
            {
                unit_return = "11111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111"
            };

            // Assert
            Assert.IsTrue(ValidateModel(model).Where(x => x.ErrorMessage.Equals("Đơn vị trả phảri dưới 100 ký tự.")).Count() > 0);
        }



    }
}
