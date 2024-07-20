using System.Globalization;
using DocsManager.Models.Dto;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace DocsManager.Services.DocsGenerator;

public class PdfGenerator : IPdfGenerator
{
    public byte[] GenerateInvoicePdf(InvoiceDto invoiceDto)
    {
        var document = CreateDocument(invoiceDto);
        return document.GeneratePdf();
    }

    private void CreateHeader(PageDescriptor header, int seriesNumber, string initials, bool isVat)
    {
        var pvm = isVat ? "PVM" : string.Empty;
        var topText = $"{initials.ToUpper()}{pvm}";
        header
            .Header()
            .Text(x =>
            {
                x.DefaultTextStyle(style => style.FontSize(10));
                x.AlignCenter();
                x.Line("SĄSKAITA FAKTŪRA");
                x.Span("Serija ");
                x.Span(topText).Bold();
                x.Line(" Nr. " + seriesNumber);
            });
    }

    private IDocument CreateDocument(InvoiceDto invoiceDto)
    {
        var isVat = invoiceDto.UserVatCode != null;
        return Document.Create(document =>
            document.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                page.DefaultTextStyle(style => style.FontSize(8).FontFamily("Arial"));
                CreateHeader(page, invoiceDto.SeriesNumber, invoiceDto.NameWithInitials, isVat);
                page.Content()
                    .Column(column =>
                    {
                        column.Spacing(40);
                        column.Item().Text(text =>
                        {
                            text.Span("Išrašymo data: ").Bold();
                            text.Line(invoiceDto.Date);
                            text.AlignRight();
                        });
                        column.Item().Row(row =>
                        {
                            row.RelativeItem().AlignLeft().Column(sellerColumn =>
                            {
                                sellerColumn
                                    .Item()
                                    .Text("PARDAVĖJAS:")
                                    .Bold();
                                sellerColumn
                                    .Item()
                                    .Text(invoiceDto.Name)
                                    .Bold();
                                sellerColumn
                                    .Item()
                                    .Text("Adresas: " + invoiceDto.Address);
                                sellerColumn
                                    .Item()
                                    .Text("Asmens kodas: " + invoiceDto.PersonalId);

                                if (isVat)
                                    sellerColumn
                                        .Item()
                                        .Text("PVM mokėtojo kodas: " + invoiceDto.UserVatCode);
                                else
                                    sellerColumn
                                        .Item()
                                        .Text("Ne PVM mokėtojas");

                                sellerColumn
                                    .Item()
                                    .Text("Veikiantis pagal individualios veiklos vykdymo pažymą");
                                sellerColumn
                                    .Item()
                                    .Text("Nr. " + invoiceDto.FreelanceWorkId);
                                sellerColumn
                                    .Item()
                                    .Text("A. s.: " + invoiceDto.BankNumber);
                                sellerColumn
                                    .Item()
                                    .Text(invoiceDto.BankName);
                            });
                            row.RelativeItem().AlignRight().Column(buyerColumn =>
                            {
                                buyerColumn
                                    .Item()
                                    .Text("PIRKĖJAS:")
                                    .Bold();
                                buyerColumn
                                    .Item()
                                    .Text(invoiceDto.BuyerName)
                                    .Bold();
                                buyerColumn
                                    .Item()
                                    .Text("Adresas: " + invoiceDto.BuyerAddress);
                                buyerColumn
                                    .Item()
                                    .Text("Įmonės kodas: " + invoiceDto.BuyerCode);
                                if (invoiceDto.VatCode != null)
                                    buyerColumn
                                        .Item()
                                        .Text("Įmonės PVM kodas: " + invoiceDto.VatCode);
                            });
                        });


                        column.Item().BorderTop(2).BorderBottom(isVat ? 0 : 1).Table(table =>
                        {
                            IContainer DefaultCellStyle(IContainer container)
                            {
                                return container
                                    .PaddingVertical(5)
                                    .PaddingHorizontal(10)
                                    .AlignCenter()
                                    .AlignMiddle();
                            }

                            IContainer CellStyle(IContainer container)
                            {
                                return DefaultCellStyle(container.BorderTop(1));
                            }


                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn();
                                columns.ConstantColumn(180);
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                                columns.ConstantColumn(75);
                                columns.RelativeColumn();
                            });

                            table.Header(header =>
                            {
                                header.Cell().Element(DefaultCellStyle).Text("Eil.\nNr.").Bold();
                                header.Cell().Element(DefaultCellStyle).Text("Prekės arba paslaugos pavadinimas")
                                    .Bold();
                                header.Cell().Element(DefaultCellStyle).Text("Matav.\nvnt.").Bold();
                                header.Cell().Element(DefaultCellStyle).Text("Kiekis").Bold();
                                header.Cell().Element(DefaultCellStyle).Text("Kaina\n(EUR)").Bold();
                                header.Cell().Element(DefaultCellStyle).Text("Suma\n(EUR)").Bold();
                            });

                            for (var i = 0; i < invoiceDto.Products.Count; i++)
                            {
                                table.Cell().Element(CellStyle).Text(i + 1 + ".");
                                table.Cell().Element(CellStyle).Text(invoiceDto.Products[i].Name);
                                table.Cell().Element(CellStyle).Text(invoiceDto.Products[i].UnitOfMeasurement);
                                table.Cell().Element(CellStyle).Text(invoiceDto.Products[i].Units.ToString());
                                table.Cell().Element(CellStyle).Text(invoiceDto.Products[i].PriceOfUnit
                                    .ToString("N2", CultureInfo.CreateSpecificCulture("lt-LT")));
                                table.Cell().Element(SumCellStyle)
                                    .Text((invoiceDto.Products[i].PriceOfUnit * invoiceDto.Products[i].Units).ToString(
                                        "N2", CultureInfo.CreateSpecificCulture("lt-LT")));
                            }

                            if (isVat)
                                table.Footer(footer =>
                                {
                                    footer.Cell().ColumnSpan(4).BorderTop(1);
                                    footer
                                        .Cell()
                                        .Element(CellStyle)
                                        .Text("Be PVM")
                                        .Bold();
                                    footer
                                        .Cell()
                                        .BorderLeft(1)
                                        .BorderTop(1)
                                        .BorderRight(1)
                                        .Element(CellStyle)
                                        .Text(invoiceDto.TotalWithoutVat);
                                    footer.Cell().ColumnSpan(4);
                                    footer
                                        .Cell()
                                        .Element(DefaultCellStyle)
                                        .Text($"PVM {invoiceDto.VatRate * 100} proc.")
                                        .Bold();
                                    footer
                                        .Cell()
                                        .Border(1)
                                        .Element(CellStyle)
                                        .Text(invoiceDto.Pvm);
                                    footer.Cell().ColumnSpan(4);
                                    footer
                                        .Cell()
                                        .Element(DefaultCellStyle)
                                        .Text("Suma iš viso")
                                        .Bold();
                                    footer
                                        .Cell()
                                        .Border(1)
                                        .Element(CellStyle)
                                        .Text(invoiceDto.TotalMoney)
                                        .Bold();
                                });

                            return;

                            IContainer SumCellStyle(IContainer container)
                            {
                                return DefaultCellStyle(isVat ? container.Border(1) : container.BorderTop(1));
                            }
                        });
                        column.Item().Row(row =>
                        {
                            row.RelativeItem().Text("Suma žodžiais: " + invoiceDto.SumInWords);
                            if (!isVat)
                                row.RelativeItem().AlignRight().Text("Suma iš viso:  " + invoiceDto.TotalMoney + " EUR")
                                    .Bold();
                        });

                        column.Item().Column(columnDescriptor =>
                        {
                            columnDescriptor.Item().Text("Sąskaitą išrašė: " + invoiceDto.Name).FontSize(6);
                            columnDescriptor.Item().PaddingVertical(1).LineHorizontal(1).LineColor(Colors.Grey.Medium);
                            columnDescriptor.Item()
                                .Text(
                                    "(asmens, atsakingo už ūkinės operacijos atlikimą ir teisingą įforminimą, pareigos, vardas, pavardė, parašas)")
                                .FontSize(6);
                        });

                        column.Item().Column(columnDescriptor =>
                        {
                            columnDescriptor.Item().Text("Sąskaitą gavo:").FontSize(6);
                            columnDescriptor.Item().PaddingVertical(1).LineHorizontal(1).LineColor(Colors.Grey.Medium);
                        });
                    });
            })
        );
    }
}