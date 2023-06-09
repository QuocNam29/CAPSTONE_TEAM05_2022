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
    public class sale_ControllerTest
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
        public void Check_False_If_Customer_ID_Value_bigger_than_0_Test()
        {
            // Arrange
            var controller = new salesController();
            var db = new CP25Team05Entities();

            // Act          
            var model = new sale()
            {
                customer_id = -2
            };

            // Assert
            Assert.IsTrue(ValidateModel(model).Where(x => x.ErrorMessage.Count() > 0).Count() > 0);
        }
        [TestMethod()]
        public void Check_False_If_Total_Value_bigger_than_0_Test()
        {
            // Arrange
            var controller = new salesController();
            var db = new CP25Team05Entities();

            // Act          
            var model = new sale()
            {
                total = -2
            };

            // Assert
            Assert.IsTrue(ValidateModel(model).Where(x => x.ErrorMessage.Count() > 0).Count() > 0);
        }
        [TestMethod()]
        public void Check_False_If_Created_by_Value_bigger_than_0_Test()
        {
            // Arrange
            var controller = new salesController();
            var db = new CP25Team05Entities();

            // Act          
            var model = new sale()
            {
                created_by = null
            };

            // Assert
            Assert.IsTrue(ValidateModel(model).Where(x => x.ErrorMessage.Equals("Vui lòng chọn ngày tạo.")).Count() > 0);
        }

        [TestMethod()]
        public void Check_False_If_Code_Null_Test()
        {
            // Arrange
            var controller = new salesController();
            var db = new CP25Team05Entities();

            // Act          
            var model = new sale()
            {
                code = null
            };

            // Assert
            Assert.IsTrue(ValidateModel(model).Where(x => x.ErrorMessage.Count() > 0).Count() > 0);
        }

        [TestMethod()]
        public void Check_False_If_code_MoreThan_20_Characters_Test()
        {
            // Arrange
            var controller = new salesController();
            var db = new CP25Team05Entities();

            // Act          
            var model = new sale()
            {
                code = "123456789101112131415161718"
            };

            // Assert
            Assert.IsTrue(ValidateModel(model).Where(x => x.ErrorMessage.Equals("Nhập mã bán hàng phải dưới 20 ký tự.")).Count() > 0);
        }
    }
}
