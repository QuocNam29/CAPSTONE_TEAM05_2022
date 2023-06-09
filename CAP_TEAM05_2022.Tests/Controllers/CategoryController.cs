using CAP_TEAM05_2022.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;



namespace CAP_TEAM05_2022.Controllers.Tests
{
    [TestClass()]
    public class CategoryControllerTest
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
        public void Check_False_If_Name_Null_Test()
        {
            // Arrange
            var controller = new categoriesController();
            var db = new CP25Team05Entities();

            // Act          
            var model = new category()
            {
                name = null
            };

            // Assert
            Assert.IsTrue(ValidateModel(model).Where(x => x.ErrorMessage.Equals("Vui lòng nhập tên danh mục")).Count() > 0);
        }
        [TestMethod()]
        public void Check_False_If_Name_MoreThan_255_Characters_Test()
        {
            // Arrange
            var controller = new categoriesController();
            var db = new CP25Team05Entities();

            // Act          
            var model = new category()
            {
                name = "11111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111"
            };

            // Assert
            Assert.IsTrue(ValidateModel(model).Where(x => x.ErrorMessage.Equals("Nhập tên danh mục phải dưới 255 ký tự.")).Count() > 0);
        }
        [TestMethod()]
        public void Check_False_If_code_Null_Test()
        {
            // Arrange
            var controller = new categoriesController();
            var db = new CP25Team05Entities();

            // Act          
            var model = new category()
            {
                code = null
            };

            // Assert
            Assert.IsTrue(ValidateModel(model).Where(x => x.ErrorMessage.Equals("Vui lòng nhập mã danh mục")).Count() > 0);
        }
        [TestMethod()]
        public void Check_False_If_code_MoreThan_100_Characters_Test()
        {
            // Arrange
            var controller = new categoriesController();
            var db = new CP25Team05Entities();

            // Act          
            var model = new category()
            {
                code = "11111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111"
            };

            // Assert
            Assert.IsTrue(ValidateModel(model).Where(x => x.ErrorMessage.Equals("Nhập mã danh mục phải dưới 100 ký tự.")).Count() > 0);
        }
        [TestMethod()]
        public void Check_False_If_created_by_Null_Test()
        {
            // Arrange
            var controller = new categoriesController();
            var db = new CP25Team05Entities();

            // Act          
            var model = new category()
            {
                created_by = null
            };

            // Assert
            Assert.IsTrue(ValidateModel(model).Where(x => x.ErrorMessage.Equals("Vui lòng chọn ngày tạo.")).Count() > 0);
        }



    }
}