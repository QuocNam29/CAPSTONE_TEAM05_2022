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
    public class return_saleControllerTest
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
        public void Check_False_If_code_Null_Test()
        {
            // Arrange
            var controller = new return_saleController();
            var db = new CP25Team05Entities();

            // Act          
            var model = new return_sale()
            {
                code = null
            };

            // Assert
            Assert.IsTrue(ValidateModel(model).Where(x => x.ErrorMessage.Equals("Vui lòng nhập mã hoàn trả")).Count() > 0);
        }
        [TestMethod()]
        public void Check_False_If_code_MoreThan_100_Characters_Test()
        {
            // Arrange
            var controller = new return_saleController();
            var db = new CP25Team05Entities();

            // Act          
            var model = new return_sale()
            {
                code = "11111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111"
            };

            // Assert
            Assert.IsTrue(ValidateModel(model).Where(x => x.ErrorMessage.Equals("Mã hoàn trả phải dưới 100 ký tự.")).Count() > 0);
        }

        [TestMethod()]
        public void Check_False_If_method_Value_bigger_than_0__Test()
        {
            // Arrange
            var controller = new return_saleController();
            var db = new CP25Team05Entities();

            // Act          
            var model = new return_sale()
            {
                method = -2
            };

            // Assert
            Assert.IsTrue(ValidateModel(model).Where(x => x.ErrorMessage.Count() > 0).Count() > 0);
        }
    }
}
