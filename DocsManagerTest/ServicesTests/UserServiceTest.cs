using DocsManager.Models.Dto;
using DocsManager.Services.User;

namespace DocGenLibaryTest.ServicesTests;

public class UserServiceTest : BaseTest
{
    [Test]
    [NonParallelizable]
    [TestCase("40cf7e27-5de2-4251-b030-9e0335803c58", true)]
    [TestCase("40cf7e27-5de2-4251-b030-9e0335803c5a", false)]
    public async Task Test_Validates_User_From_Guid(string userId, bool expectedResult)
    {
        var uid = Guid.Parse(userId);
        var service = GetService();
        Assert.That(await service.ValidateUser(uid), Is.EqualTo(expectedResult));
    }

    [Test]
    [NonParallelizable]
    [TestCase("40cf7e27-5de2-4251-b030-9e0335803c5a", "Test", "User", true)]
    [TestCase("40cf7e27-5de2-4251-b030-9e0335803c58", "Testaaa", "Userbbb", false)]
    public async Task Test_Inserts_User(string userId, string userName, string surName, bool expectedResult)
    {
        var userPost = new UserPostDto
        {
            Address = "Test", BankNumber = "111", BankName = "BankName", FreelanceWorkId = "1123", PersonalId = "6146"
        };
        var uid = Guid.Parse(userId);
        var service = GetService();
        var result = await service.InsertUser(uid, userPost, userName, surName);
        Assert.That(result, Is.EqualTo(expectedResult));
    }

    [Test]
    [NonParallelizable]
    [TestCase("53c09e09-eea1-49b5-ab81-26ff78740b7d", "FR35H")]
    [TestCase("e255553a-1ce4-4f32-9c56-276b24096a4e", "R3AL")]
    public async Task Test_Returns_User_Info(string userId, string personalId)
    {
        var service = GetService();
        var guid = Guid.Parse(userId);
        var result = await service.GetUser(guid);
        Assert.That(result, Is.Not.Null);
        Assert.That(result.PersonalId, Is.EqualTo(personalId));
    }

    [Test]
    [NonParallelizable]
    [TestCase("53c09e09-eea1-49b5-ab81-26ff78740b7a")]
    public async Task Test_Returns_Null_On_Non_Found_User(string userId)
    {
        var service = GetService();
        var guid = Guid.Parse(userId);
        var result = await service.GetUser(guid);
        Assert.That(result, Is.Null);
    }

    private UserService GetService()
    {
        return new UserService(DbContext);
    }
}