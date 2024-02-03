using DocsManager.Models.Dto;

namespace DocGenLibaryTest;

[TestFixture]
public class InvoiceDtoTests
{
    [TestCase(123.45, "šimtas dvidešimt trys eurai, keturiasdešimt penki ct.")]
    [TestCase(678.90, "šeši šimtai septyniasdešimt aštuoni eurai, devyniasdešimt ct.")]
    [TestCase(1000, "tūkstantis eurų, 0 ct.")]
    [TestCase(987.65, "devyni šimtai aštuoniasdešimt septyni eurai, šešiasdešimt penki ct.")]
    [TestCase(1234.56, "tūkstantis du šimtai trisdešimt keturi eurai, penkiasdešimt šeši ct.")]
    [TestCase(55.75, "penkiasdešimt penki eurai, septyniasdešimt penki ct.")]
    [TestCase(0.99, "devyniasdešimt devyni ct.")]
    [TestCase(1500.50, "tūkstantis penki šimtai eurų, penkiasdešimt ct.")]
    [TestCase(101.58, "šimtas vienas euras, penkiasdešimt aštuoni ct.")]
    [TestCase(99876.54, "devyniasdešimt devyni tūkstančiai aštuoni šimtai septyniasdešimt šeši eurai, penkiasdešimt keturi ct.")]
    public void Test_Correctly_Converts_Sum_To_Words(decimal ammount, string expectedWords)
    {
        var result = InvoiceDto.ConvertSumToWords(ammount);

        Assert.AreEqual(expectedWords, result);
    }
}