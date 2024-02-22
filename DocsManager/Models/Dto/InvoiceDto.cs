using System.Collections;
using System.Globalization;
using System.Text;
using Microsoft.Extensions.Primitives;

namespace DocsManager.Models.Dto;

public class InvoiceDto
{
    public int SeriesNumber { get; set; }
    public string Date { get; set; }
    public string Name { get; set; }
    public string Address { get; set; }
    public string PersonalId { get; set; }
    public string FreelanceWorkId { get; set; }
    public string BankNumber { get; set; }
    public string BankName { get; set; }
    public string BuyerName { get; set; }
    public string BuyerAddress { get; set; }
    public string BuyerCode { get; set; }
    public string? VatCode { get; set; }
    public List<ItemDto> Products { get; set; }
    public string SumInWords => ConvertSumToWords(TotalSum);
    private decimal TotalSum => Products.Sum(product => product.TotalPrice);
    public string NameWithInitials { get; set; }
    public string TotalMoney => TotalSum.ToString("N2", CultureInfo.CreateSpecificCulture("lt-LT"));
    
    public static string ConvertSumToWords(decimal sum)
    {
        int integral = (int)sum;
        int fractionPart = (int)((sum % 1.0m) * 100);
        string cents = "0 ct.";
        string euros = "0 eurų";
        string result = euros + ", " + cents;
        
        if (fractionPart != 0)
        {
            cents = ConvertIntegerToWords(fractionPart) + "ct.";
            result = euros + ", " + cents;
        }

        if (integral != 0)
        {
            var integralInWords = ConvertIntegerToWords(integral);
            string ending;
            switch (integral % 10)
            {
                case 1:
                    ending = "euras";
                    break;
                case 0:
                    ending = "eurų";
                    break;
                default:
                    ending = "eurai";
                    break;
            }
            
            result = integralInWords + ending + ", " + cents;
        }


        return char.ToUpper(result[0]) + result.Substring(1);
    }

    private static string ConvertIntegerToWords(int integer)
    {
        var result = new StringBuilder();
        if (integer > 1999)
        {
            var thousands = integer / 1000;
            result.Append(ConvertIntegerToWords(thousands));
            string ending;
            switch (thousands % 10)
            {
                case 1:
                    ending = "tūkstantis";
                    break;
                case 0:
                    ending = "tūkstančių";
                    break;
                default:
                    ending = "tūkstančiai";
                    break;
            }

            result.Append(ending + " ");
            integer = integer - thousands * 1000;
        }
        else if (integer > 999)
        {
            result.Append("tūkstantis ");
            integer = integer - 1000;
        }

        if (integer > 199)
        {
            var hundreds = integer / 100;
            result.Append(ConvertIntegerToWords(hundreds) + "šimtai ");
            integer = integer - hundreds * 100;
        }
        else if (integer > 99)
        {
            result.Append("šimtas ");
            integer = integer - 100;
        }

        if (integer > 19)
        {
            var tens = integer / 10;
            string tensInWords;
            switch (tens)
            {
                case 2:
                    tensInWords = "dvidešimt";
                    break;
                case 3:
                    tensInWords = "trisdešimt";
                    break;
                case 4:
                    tensInWords = "keturiasdešimt";
                    break;
                case 5:
                    tensInWords = "penkiasdešimt";
                    break;
                case 6:
                    tensInWords = "šešiasdešimt";
                    break;
                case 7:
                    tensInWords = "septyniasdešimt";
                    break;
                case 8:
                    tensInWords = "aštuoniasdešimt";
                    break;
                case 9:
                    tensInWords = "devyniasdešimt";
                    break;
                default:
                    throw new Exception("Invalid tens");
            }

            result.Append(tensInWords + " ");
            integer = integer - tens * 10;
        }
        else if (integer > 9)
        {
            string aboveTen;
            switch (integer)
            {
                case 10:
                    aboveTen = "dešimt";
                    break;
                case 11:
                    aboveTen = "vienuolika";
                    break;
                case 12:
                    aboveTen = "dvylika";
                    break;
                case 13:
                    aboveTen = "trylika";
                    break;
                case 14:
                    aboveTen = "keturiolika";
                    break;
                case 15:
                    aboveTen = "penkiolika";
                    break;
                case 16:
                    aboveTen = "šešiolika";
                    break;
                case 17:
                    aboveTen = "septyniolika";
                    break;
                case 18:
                    aboveTen = "aštoniolika";
                    break;
                case 19:
                    aboveTen = "devyniolika";
                    break;
                default:
                    throw new Exception("Invalid above 10s");
            }

            result.Append(aboveTen + " ");
            integer = 0;
        }

        if (integer > 0)
        {
            string digitInWords;
            switch (integer)
            {
                case 1:
                    digitInWords = "vienas";
                    break;
                case 2:
                    digitInWords = "du";
                    break;
                case 3:
                    digitInWords = "trys";
                    break;
                case 4:
                    digitInWords = "keturi";
                    break;
                case 5:
                    digitInWords = "penki";
                    break;
                case 6:
                    digitInWords = "šeši";
                    break;
                case 7:
                    digitInWords = "septyni";
                    break;
                case 8:
                    digitInWords = "aštuoni";
                    break;
                case 9:
                    digitInWords = "devyni";
                    break;
                default:
                    throw new Exception("Invalid digit");
            }

            result.Append(digitInWords + " ");
        }

        return result.ToString();
    }
}