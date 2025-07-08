using aluguel_de_imoveis_wpf.Model;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Collections.Generic;

namespace aluguel_de_imoveis_wpf.Relatorios
{
    public class RelatorioDeImoveisDisponiveis : IDocument
    {
        private readonly List<Imovel> _dados;

        public RelatorioDeImoveisDisponiveis(List<Imovel> imovel)
        {
            _dados = imovel;
        }

        public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

        public void Compose(IDocumentContainer container)
        {
            container.Page(page =>
            {
                page.Margin(30);
                page.Size(PageSizes.A4.Landscape());
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(10));

                page.Header()
                    .Text("Relatório de Imóveis Disponíveis")
                    .SemiBold().FontSize(16).FontColor(Colors.Blue.Medium).AlignCenter();

                page.Content()
                    .Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(2);
                            columns.RelativeColumn(2); 
                            columns.RelativeColumn(2); 
                            columns.RelativeColumn(2); 
                            columns.RelativeColumn(1);
                            columns.RelativeColumn(1); 
                        });

                        table.Header(header =>
                        {
                            header.Cell().Element(CellStyle).Text("Proprietário").Bold();
                            header.Cell().Element(CellStyle).Text("Contato").Bold();
                            header.Cell().Element(CellStyle).Text("Telefone").Bold();
                            header.Cell().Element(CellStyle).Text("Título Imóvel").Bold();
                            header.Cell().Element(CellStyle).Text("Tipo").Bold();
                            header.Cell().Element(CellStyle).Text("Valor Aluguel").Bold();
                        });

                        foreach (var item in _dados)
                        {
                            table.Cell().Element(CellStyle).Text(item.Usuario.Nome);
                            table.Cell().Element(CellStyle).Text(item.Usuario.Email);
                            table.Cell().Element(CellStyle).Text(item.Usuario.Telefone);
                            table.Cell().Element(CellStyle).Text(item.Titulo);
                            table.Cell().Element(CellStyle).Text(item.Tipo.ToString());
                            table.Cell().Element(CellStyle).Text($"R$ {item.ValorAluguel:N2}");
                        }

                        static IContainer CellStyle(IContainer container)
                        {
                            return container
                                .Border(0.5f)
                                .BorderColor(Colors.Grey.Lighten2)
                                .Padding(5);
                        }
                    });
            });
        }
    }
}