using DemoCRUDApi.Controllers;
using DemoCRUDApi.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoCRUDApiUnitTest
{
    public class DemoeControllerTest
    {
        private readonly Mock<DemoDBContext> MockContext;

        public DemoeControllerTest()
        {
            MockContext = new Mock<DemoDBContext>();
        }

        [Fact]
        public async Task GetCrudDemo_HasData_Equal()
        {
            var testData = new CrudDemo[]
            {
                new CrudDemo { Id = 1, DemoName = "AAA" },
                new CrudDemo { Id = 2, DemoName = "BBB" },
                new CrudDemo { Id = 3, DemoName = "CCC" },
            };
            var queryableTestData = testData.AsQueryable();

            var mockDbSet = new Mock<DbSet<CrudDemo>>();
            mockDbSet.As<IQueryable<CrudDemo>>().Setup(m => m.Provider).Returns(queryableTestData.Provider);
            mockDbSet.As<IQueryable<CrudDemo>>().Setup(m => m.Expression).Returns(queryableTestData.Expression);
            mockDbSet.As<IQueryable<CrudDemo>>().Setup(m => m.ElementType).Returns(queryableTestData.ElementType);
            mockDbSet.As<IQueryable<CrudDemo>>().Setup(m => m.GetEnumerator()).Returns(queryableTestData.GetEnumerator());
            mockDbSet.As<IAsyncEnumerable<CrudDemo>>().Setup(x => x.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
               .Returns(new FakeAsyncEnumerator<CrudDemo>(queryableTestData.GetEnumerator()));

            MockContext.Setup(c => c.CrudDemo).Returns(mockDbSet.Object);

            var controller = new DemoeController(MockContext.Object);

            var response = await controller.GetCrudDemo();

            Assert.Null(response.Result);
            Assert.Equal(3, response.Value!.Count());
            var result = response.Value!.ToArray();
            for (int i = 0; i < testData.Length; i++)
            {
                Assert.Equal(testData[i], result[i]);
            }
        }

        [Fact]
        public async Task GetCrudDemo_DataSetIsNull_ReturnsNotFound()
        {
            DataSetSetupNull();
            var controller = new DemoeController(MockContext.Object);
            var response = await controller.GetCrudDemo();

            Assert.IsType<NotFoundResult>(response.Result);
        }

        [Fact]
        public async Task GetCrudDemoById_HasData_Equal()
        {
            var testData = new CrudDemo[]
            {
                new CrudDemo { Id = 1, DemoName = "AAA" },
                new CrudDemo { Id = 2, DemoName = "BBB" },
                new CrudDemo { Id = 3, DemoName = "CCC" },
            };

            var mockDbSet = new Mock<DbSet<CrudDemo>>();
            for (int i = 0; i < testData.Length; i++)
            {
                mockDbSet.Setup(s => s.FindAsync(testData[i].Id)).ReturnsAsync(testData[i]);
            }

            MockContext.Setup(c => c.CrudDemo).Returns(mockDbSet.Object);

            var controller = new DemoeController(MockContext.Object);

            for (int i = 0; i < testData.Length; i++)
            {
                var response = await controller.GetCrudDemo(testData[i].Id);

                Assert.Null(response.Result);
                Assert.Equal(testData[i], response.Value!);
            }
        }

        [Fact]
        public async Task GetCrudDemoById_DataSetIsNullOrNotExist_ReturnsNotFound()
        {
            DataSetSetupNull();
            var controller = new DemoeController(MockContext.Object);

            var response = await controller.GetCrudDemo(0);
            Assert.IsType<NotFoundResult>(response.Result);

            var mockDbSet = new Mock<DbSet<CrudDemo>>();
            mockDbSet.Setup(s => s.FindAsync(It.IsAny<int>())).ReturnsAsync((CrudDemo)null);

            MockContext.Setup(c => c.CrudDemo).Returns(mockDbSet.Object);

            response = await controller.GetCrudDemo(0);
            Assert.IsType<NotFoundResult>(response.Result);
        }

        [Fact]
        public async Task PutCrudDemo_WhenIdDiffOrNotExist_ReturnsBadRequestOrNotFound()
        {
            var aaa = new CrudDemo { Id = 1, DemoName = "AAA" };
            var bbb = new CrudDemo { Id = 2, DemoName = "AAA" };
            
            var mockDbSet = new Mock<DbSet<CrudDemo>>();
            mockDbSet.Setup(s => s.FindAsync(aaa.Id)).ReturnsAsync(aaa);
            MockContext.Setup(c => c.CrudDemo).Returns(mockDbSet.Object);

            var controller = new DemoeController(MockContext.Object);
            var result = await controller.PutCrudDemo(aaa.Id, bbb);
            Assert.IsType<BadRequestResult>(result);

            result = await controller.PutCrudDemo(bbb.Id, bbb);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task PutCrudDemo_WhenCrudDemoExists_ReturnsNoContentResult()
        {
            var id = 1;
            var existItem = new CrudDemo { Id = id, DemoName = "AAA" }; 
            
            var mockDbSet = new Mock<DbSet<CrudDemo>>();
            mockDbSet.Setup(s => s.FindAsync(id)).ReturnsAsync(existItem);
            MockContext.Setup(c => c.CrudDemo).Returns(mockDbSet.Object);

            var controller = new DemoeController(MockContext.Object);
            var putItem = new CrudDemo { Id = id, DemoName = "BBB" };
            MockContext.Setup(c => c.SetModified(It.IsAny<object>()));

            var result = await controller.PutCrudDemo(id, putItem);

            Assert.IsType<NoContentResult>(result);
            Assert.Equal(putItem.DemoName, existItem.DemoName);
        }

        private void DataSetSetupNull()
        {
#pragma warning disable CS8600 // 正在將 Null 常值或可能的 Null 值轉換為不可為 Null 的型別。
#pragma warning disable CS8625 // 無法將 null 常值轉換成不可為 Null 的參考型別。
            MockContext.Setup(c => c.CrudDemo).Returns((DbSet<CrudDemo>)null);
#pragma warning restore CS8625 // 無法將 null 常值轉換成不可為 Null 的參考型別。
#pragma warning restore CS8600 // 正在將 Null 常值或可能的 Null 值轉換為不可為 Null 的型別。
        }
    }
}
