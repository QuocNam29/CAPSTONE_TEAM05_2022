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
    public class inventory_orderController

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
            var controller = new inventory_orderController();
            var db = new CP25Team05Entities();

            // Act          
            var model = new inventory_order()
            {
                code = null
            };

            // Assert
            Assert.IsTrue(ValidateModel(model).Where(x => x.ErrorMessage.Equals("Vui lòng nhập mã đơn hàng tồn kho.")).Count() > 0);
        }
        [TestMethod()]
        public void Check_False_If_code_MoreThan_50_Characters_Test()
        {
            // Arrange
            var controller = new inventory_orderController();
            var db = new CP25Team05Entities();

            // Act          
            var model = new inventory_order()
            {
                code = "11111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111"
            };

            // Assert
            Assert.IsTrue(ValidateModel(model).Where(x => x.ErrorMessage.Equals("Nhập mã đơn hàng tồn kho phải dưới 100 ký tự.")).Count() > 0);
        }

        [TestMethod()]
        public void Check_False_If_Supplier_id_Null_Test()
        {
            // Arrange
            var controller = new inventory_orderController();
            var db = new CP25Team05Entities();

            // Act          
            var model = new inventory_order()
            {
                supplier_id = -2
            };

            // Assert
            Assert.IsTrue(ValidateModel(model).Where(x => x.ErrorMessage.Count() > 0).Count() > 0);

        }

        [TestMethod()]
        public void Check_False_If_Create_by_Null_Test()
        {
            // Arrange
            var controller = new inventory_orderController();
            var db = new CP25Team05Entities();

            // Act          
            var model = new inventory_order()
            {
                create_by = null
            };

            // Assert
            Assert.IsTrue(ValidateModel(model).Where(x => x.ErrorMessage.Equals("Vui lòng chọn ngày tạo.")).Count() > 0);
        }

        [TestMethod()]
        public void Check_False_If_Total_Value_bigger_than_0_Test()
        {
            // Arrange
            var controller = new inventory_orderController();
            var db = new CP25Team05Entities();

            // Act          
            var model = new inventory_order()
            {
                Total = -2
            };

            // Assert
            Assert.IsTrue(ValidateModel(model).Where(x => x.ErrorMessage.Equals("Vui lòng nhập giá trị lớn hơn 0")).Count() > 0);

        }
    }
}
