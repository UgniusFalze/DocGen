using System.Globalization;
using DocsManager.Controllers.Types;
using DocsManager.Models.Dto;
using DocsManager.Services.Errors;
using DocsManager.Services.IntegerToWordsConverter;
using DocsManager.Services.Invoice;
using DocsManager.Services.User;

namespace DocGenLibaryTest.ServicesTests;

public class InvoiceServiceTest : BaseTest
{
    [Test]
    [NonParallelizable]
    [TestCase("e255553a-1ce4-4f32-9c56-276b24096a4e", 32716.7, new[] { 1, 2 }, 0)]
    [TestCase("e255553a-1ce4-4f32-9c56-276b24096a4e", 0, new int[0], 1)]
    public async Task Test_Gets_Users_Mapped_Invoices(string userId, decimal totalSum, int[] seriesNumbers, int page)
    {
        var guid = Guid.Parse(userId);
        var service = GetService();
        var result = await service.GetInvoiceForGrid(guid, page);
        Assert.Multiple(() =>
        {
            Assert.That(result.InvoicesTotal, Is.EqualTo(totalSum));
            Assert.That(result.Invoices.Select(invoice => invoice.InvoiceId), Is.EqualTo(seriesNumbers));
        });
    }

    [Test]
    [NonParallelizable]
    [TestCase("40cf7e27-5de2-4251-b030-9e0335803c58", 5)]
    [TestCase("e255553a-1ce4-4f32-9c56-276b24096a4e", 2)]
    [TestCase("53c09e09-eea1-49b5-ab81-26ff78740b7d", 0)]
    public async Task Test_Gets_Users_Invoice_Total_Count(string userId, int count)
    {
        var service = GetService();
        var guid = Guid.Parse(userId);
        var result = await service.GetInvoiceCount(guid);
        Assert.That(result, Is.EqualTo(count));
    }

    [Test]
    [NonParallelizable]
    [TestCase(4, "40cf7e27-5de2-4251-b030-9e0335803c58", "0,09", "In the test container")]
    [TestCase(2, "e255553a-1ce4-4f32-9c56-276b24096a4e", "6,05", "In the real world",
        Description = "User with vat code")]
    public async Task Test_Gets_Invoice_With_Id_And_User(int id, string userId, string totalMoney, string address)
    {
        var guid = Guid.Parse(userId);
        var service = GetService();
        var user = new BearerUser(guid, "Test", "User");
        var result = await service.GetInvoice(id, user);
        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result.SeriesNumber, Is.EqualTo(id));
            Assert.That(result.TotalMoney, Is.EqualTo(totalMoney));
            Assert.That(result.Address, Is.EqualTo(address));
            Assert.That(result.NameWithInitials, Is.EqualTo("TU"));
        });
    }

    [Test]
    [NonParallelizable]
    [TestCase(5, "e255553a-1ce4-4f32-9c56-276b24096a4e")]
    [TestCase(3, "e255553a-1ce4-4f32-9c56-276b24096a4e")]
    [TestCase(33, "40cf7e27-5de2-4251-b030-9e0335803c58")]
    [TestCase(-1, "40cf7e27-5de2-4251-b030-9e0335803c58")]
    [TestCase(2, "40cf7e27-5de2-4251-b030-9e0335803c59")]
    public async Task Test_Returns_Null_When_User_Or_Series_Number_Is_Not_Found(int id, string userId)
    {
        var guid = Guid.Parse(userId);
        var user = new BearerUser(guid, "Test", "User");
        var service = GetService();
        var result = await service.GetInvoice(id, user);

        Assert.That(result, Is.Null);
    }

    [Test]
    [NonParallelizable]
    [TestCase("e255553a-1ce4-4f32-9c56-276b24096a4e", 1, 3)]
    public async Task Test_Inserts_Invoice_And_Items_For_User(string userId, int clientId,
        int seriesNumber)
    {
        var invoicePost = new InvoicePostDto
        {
            ClientId = clientId, InvoiceDate = DateTime.Now.ToString(CultureInfo.CurrentCulture),
            SeriesNumber = seriesNumber, Items = new List<ItemPostDto>()
        };
        var guid = Guid.Parse(userId);
        var service = GetService();
        var result = await service.InsertInvoice(invoicePost, guid);
        Assert.That(result.IsFailed, Is.False);
        Assert.Multiple(() =>
        {
            Assert.That(result.Value.InvoiceClientId, Is.EqualTo(clientId));
            Assert.That(result.Value.SeriesNumber, Is.EqualTo(seriesNumber));
            Assert.That(result.Value.InvoiceUserId, Is.EqualTo(guid));
        });
    }

    [Test]
    [NonParallelizable]
    [TestCase("e255553a-1ce4-4f32-9c56-276b24096a4e", 99, 3)]
    [TestCase("e255553a-1ce4-4f32-9c56-276b24096a4a", 1, 3)]
    public async Task Test_Invoice_Insert_Returns_Null_On_Not_Found(string userId, int clientId,
        int seriesNumber)
    {
        var invoicePost = new InvoicePostDto
        {
            ClientId = clientId, InvoiceDate = DateTime.Now.ToString(CultureInfo.CurrentCulture),
            SeriesNumber = seriesNumber, Items = new List<ItemPostDto>()
        };
        var guid = Guid.Parse(userId);
        var service = GetService();
        var result = await service.InsertInvoice(invoicePost, guid);
        Assert.That(result.IsFailed, Is.True);
        var error = result.Errors.First();
        var expectedError = new NotFoundError("User or client");
        Assert.Multiple(() =>
            {
                Assert.That(error.Message, Is.EqualTo(expectedError.Message));
                Assert.That(error.Metadata, Is.EqualTo(expectedError.Metadata));
            }
        );
    }

    [Test]
    [NonParallelizable]
    [TestCase("e255553a-1ce4-4f32-9c56-276b24096a4e", 1, 1)]
    [TestCase("40cf7e27-5de2-4251-b030-9e0335803c58", 1, 1)]
    public async Task Test_Invoice_Insert_Returns_Duplication_On_Duplicate(string userId, int clientId,
        int seriesNumber)
    {
        var invoicePost = new InvoicePostDto
        {
            ClientId = clientId, InvoiceDate = DateTime.Now.ToString(CultureInfo.CurrentCulture),
            SeriesNumber = seriesNumber, Items = new List<ItemPostDto>()
        };
        var guid = Guid.Parse(userId);
        var service = GetService();
        var result = await service.InsertInvoice(invoicePost, guid);
        Assert.That(result.IsFailed, Is.True);
        var error = result.Errors.First();
        var expectedError = new DuplicationError("Invoice", "series number");
        Assert.Multiple(() =>
            {
                Assert.That(error.Message, Is.EqualTo(expectedError.Message));
                Assert.That(error.Metadata, Is.EqualTo(expectedError.Metadata));
            }
        );
    }


    [Test]
    [NonParallelizable]
    [TestCase("e255553a-1ce4-4f32-9c56-276b24096a4e", 1, true)]
    [TestCase("40cf7e27-5de2-4251-b030-9e0335803c58", 5, true)]
    [TestCase("e255553a-1ce4-4f32-9c56-276b24096a4e", 6, false)]
    [TestCase("40cf7e27-5de2-4251-b030-9e0335803c58", 88, false)]
    [TestCase("40cf7e27-5de2-4251-b030-9e0335803c5a", 1, false)]
    public async Task Test_Deletes_Invoice_From_User(string userId, int id, bool expectedResult)
    {
        var guid = Guid.Parse(userId);
        var service = GetService();
        var result = await service.DeleteInvoice(id, guid);
        Assert.That(result, Is.EqualTo(expectedResult));
    }

    [Test]
    [NonParallelizable]
    [TestCase("e255553a-1ce4-4f32-9c56-276b24096a4e", 2)]
    [TestCase("40cf7e27-5de2-4251-b030-9e0335803c58", 5)]
    [TestCase("40cf7e27-5de2-4251-b030-9e0335803c5a", null)]
    [TestCase("53c09e09-eea1-49b5-ab81-26ff78740b7d", null)]
    public async Task Test_Gets_Latest_User_Invoice_Id(string userId, int? expectedResult)
    {
        var guid = Guid.Parse(userId);
        var service = GetService();
        var result = await service.GetLatestUserInvoice(guid);
        Assert.That(result, Is.EqualTo(expectedResult));
    }

    [Test]
    [NonParallelizable]
    [TestCase(2, "e255553a-1ce4-4f32-9c56-276b24096a4e", true)]
    [TestCase(99, "e255553a-1ce4-4f32-9c56-276b24096a4e", false)]
    [TestCase(5, "40cf7e27-5de2-4251-b030-9e0335803c58", true)]
    [TestCase(5, "40cf7e27-5de2-4251-b030-9e0335803c5a", false)]
    public async Task Test_Adds_Item_To_Invoice(int id, string userId, bool expectedResult)
    {
        var itemPost = new ItemPostDto { Name = "Test", PriceOfUnit = 1, UnitOfMeasurement = "vnt", Units = 1 };
        var guid = Guid.Parse(userId);
        var service = GetService();
        var result = await service.AddItemToInvoice(id, itemPost, guid);
        Assert.That(result, Is.EqualTo(expectedResult));
    }

    [Test]
    [NonParallelizable]
    [TestCase(1, true, "40cf7e27-5de2-4251-b030-9e0335803c58", true)]
    [TestCase(55, true, "40cf7e27-5de2-4251-b030-9e0335803c58", false)]
    [TestCase(55, false, "e255553a-1ce4-4f32-9c56-276b24096a4e", false)]
    [TestCase(2, false, "e255553a-1ce4-4f32-9c56-276b24096a4e", true)]
    [TestCase(2, false, "e255553a-1ce4-4f32-9c56-276b24096a4a", false)]
    public async Task Test_Sets_Payed_For_Invoice(int id, bool hasPayed, string userId, bool expectedResult)
    {
        var guid = Guid.Parse(userId);
        var isPayed = new IsPayedDto(hasPayed);
        var service = GetService();
        var result = await service.SetPayed(id, isPayed, guid);
        Assert.That(result, Is.EqualTo(expectedResult));
    }

    [Test]
    [NonParallelizable]
    [TestCase(2, 5, "e255553a-1ce4-4f32-9c56-276b24096a4e", true)]
    [TestCase(4, 6, "40cf7e27-5de2-4251-b030-9e0335803c58", true)]
    [TestCase(2, 5, "40cf7e27-5de2-4251-b030-9e0335803c58", false)]
    [TestCase(4, 5, "40cf7e27-5de2-4251-b030-9e0335803c58", false)]
    [TestCase(4, 5, "40cf7e27-5de2-4251-b030-9e0335803c5a", false)]
    [TestCase(2, 9, "e255553a-1ce4-4f32-9c56-276b24096a4e", false)]
    public async Task Test_Removes_Item_From_Invoice(int id, int invoiceItemId, string userId, bool expectedResult)
    {
        var guid = Guid.Parse(userId);
        var service = GetService();
        var result = await service.RemoveItemFromInvoice(id, invoiceItemId, guid);
        Assert.That(result, Is.EqualTo(expectedResult));
    }

    [Test]
    [NonParallelizable]
    [TestCase(1, "40cf7e27-5de2-4251-b030-9e0335803c58", "2024-06-17", 1, 6)]
    [TestCase(2, "e255553a-1ce4-4f32-9c56-276b24096a4e", "2024-12-31", 4, 3)]
    public async Task Test_Updates_Invoice(int invoiceId, string userId, string invoiceDate, int clientId,
        int seriesNumber)
    {
        var guid = Guid.Parse(userId);
        var service = GetService();
        var post = new InvoiceUpdatePost
            { InvoiceClientId = clientId, InvoiceDate = invoiceDate, SeriesNumber = seriesNumber };
        var result = await service.UpdateInvoice(post, invoiceId, guid);
        Assert.That(result.IsSuccess, Is.True);
        var updated = await service.GetInvoice(seriesNumber, new BearerUser(guid, "test", "test"));
        Assert.Multiple(() =>
        {
            Assert.That(updated.SeriesNumber, Is.EqualTo(seriesNumber));
            Assert.That(updated.Date, Is.EqualTo(DateTime.Parse(invoiceDate).ToUniversalTime().ToString("yyyy MM dd")));
        });
    }

    private InvoiceService GetService()
    {
        return new InvoiceService(DbContext, new LithuanianIntegerToWords(), new UserService(DbContext));
    }
}