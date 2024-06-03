using DocsManager.Models.Dto;

namespace DocGenLibaryTest;

[TestFixture]
public class InvoiceDtoTests
{
    [TestCase(123.45, "Šimtas dvidešimt trys eurai, keturiasdešimt penki ct.")]
    [TestCase(678.90, "Šeši šimtai septyniasdešimt aštuoni eurai, devyniasdešimt ct.")]
    [TestCase(1000, "Tūkstantis eurų, 0 ct.")]
    [TestCase(987.65, "Devyni šimtai aštuoniasdešimt septyni eurai, šešiasdešimt penki ct.")]
    [TestCase(1234.56, "Tūkstantis du šimtai trisdešimt keturi eurai, penkiasdešimt šeši ct.")]
    [TestCase(55.75, "Penkiasdešimt penki eurai, septyniasdešimt penki ct.")]
    [TestCase(0.99, "0 eurų, devyniasdešimt devyni ct.")]
    [TestCase(1500.50, "Tūkstantis penki šimtai eurų, penkiasdešimt ct.")]
    [TestCase(101.58, "Šimtas vienas euras, penkiasdešimt aštuoni ct.")]
    [TestCase(99876.54,
        "Devyniasdešimt devyni tūkstančiai aštuoni šimtai septyniasdešimt šeši eurai, penkiasdešimt keturi ct.")]
    public void Test_Correctly_Converts_Sum_To_Words(decimal ammount, string expectedWords)
    {
        var result = InvoiceDto.ConvertSumToWords(ammount);

        Assert.AreEqual(expectedWords, result);
    }
}