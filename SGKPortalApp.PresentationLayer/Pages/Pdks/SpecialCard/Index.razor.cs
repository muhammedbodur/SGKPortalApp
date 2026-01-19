using ClosedXML.Excel;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.PdksIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.PdksIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.ZKTeco;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.Enums.PdksIslemleri;
using SGKPortalApp.Common.Extensions;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.ZKTeco;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Common;
using SGKPortalApp.PresentationLayer.Services.UIServices.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SGKPortalApp.PresentationLayer.Pages.Pdks.SpecialCard
{
    public partial class Index
    {
        [Inject] private ISpecialCardApiService SpecialCardApiService { get; set; } = default!;
        [Inject] private IZKTecoDeviceApiService DeviceApiService { get; set; } = default!;
        [Inject] private IHizmetBinasiApiService HizmetBinasiApiService { get; set; } = default!;
        [Inject] private IToastService ToastService { get; set; } = default!;
        [Inject] private IJSRuntime JSRuntime { get; set; } = default!;

        private List<SpecialCardResponseDto> specialCards = new();
        private List<SpecialCardResponseDto> filteredCards = new();
        private List<SpecialCardResponseDto> pagedCards = new();
        private List<DeviceResponseDto> devices = new();
        private List<HizmetBinasiResponseDto> hizmetBinalari = new();

        private bool isLoading = true;
        private bool showAddForm = false;
        private bool showEditForm = false;
        private bool showDeviceSelectModal = false;

        // Export state
        private bool IsExporting { get; set; } = false;
        private string ExportType { get; set; } = string.Empty;

        private CardModel newCard = new() { CardType = CardType.ViziteKarti };
        private CardModel editCard = new();
        private int editingCardId = 0;
        private int selectedCardForSending = 0;
        private int selectedDeviceId = 0;

        // Filtering
        private string searchTerm = string.Empty;
        private string selectedCardType = string.Empty;

        // Pagination
        private int currentPage = 1;
        private int pageSize = 25;
        private int totalPages => (int)Math.Ceiling((double)filteredCards.Count / pageSize);

        protected override async Task OnInitializedAsync()
        {
            QuestPDF.Settings.License = LicenseType.Community;
            await LoadHizmetBinalari();
            await LoadCards();
            await LoadDevices();
        }

        private async Task LoadHizmetBinalari()
        {
            try
            {
                var result = await HizmetBinasiApiService.GetAllAsync();
                if (result.Success && result.Data != null)
                {
                    hizmetBinalari = result.Data;
                }
            }
            catch (Exception ex)
            {
                await ToastService.ShowErrorAsync($"Hizmet binaları yüklenemedi: {ex.Message}");
            }
        }

        private async Task LoadCards()
        {
            isLoading = true;
            try
            {
                var result = await SpecialCardApiService.GetAllAsync();
                if (result.Success && result.Data != null)
                {
                    specialCards = result.Data;
                    ApplyFilter();
                }
                else
                {
                    await ToastService.ShowErrorAsync(result.Message ?? "Kartlar yüklenemedi");
                }
            }
            catch (Exception ex)
            {
                await ToastService.ShowErrorAsync($"Hata: {ex.Message}");
            }
            finally
            {
                isLoading = false;
            }
        }

        private async Task LoadDevices()
        {
            try
            {
                var result = await DeviceApiService.GetActiveAsync();
                if (result.Success && result.Data != null)
                {
                    devices = result.Data;
                }
            }
            catch (Exception ex)
            {
                await ToastService.ShowErrorAsync($"Cihazlar yüklenemedi: {ex.Message}");
            }
        }

        private void ApplyFilter()
        {
            filteredCards = specialCards.ToList();

            // Card Type Filter
            if (!string.IsNullOrEmpty(selectedCardType) && Enum.TryParse<CardType>(selectedCardType, out var cardType))
            {
                filteredCards = filteredCards.Where(c => c.CardType == cardType).ToList();
            }

            // Search Filter
            if (!string.IsNullOrEmpty(searchTerm))
            {
                var term = searchTerm.ToLower();
                filteredCards = filteredCards.Where(c =>
                    c.CardName.ToLower().Contains(term) ||
                    c.EnrollNumber.ToLower().Contains(term) ||
                    c.CardNumber.ToString().Contains(term)
                ).ToList();
            }

            // Reset to first page
            currentPage = 1;
            ApplyPagination();
        }

        private void ApplyPagination()
        {
            pagedCards = filteredCards
                .Skip((currentPage - 1) * pageSize)
                .Take(pageSize)
                .ToList();
        }

        private void GoToPage(int page)
        {
            if (page < 1 || page > totalPages) return;
            currentPage = page;
            ApplyPagination();
        }

        private void ClearFilters()
        {
            searchTerm = string.Empty;
            selectedCardType = string.Empty;
            ApplyFilter();
        }

        private void ToggleAddForm()
        {
            showAddForm = !showAddForm;
            if (showAddForm)
            {
                newCard = new CardModel();
                showEditForm = false;
            }
        }

        private void ToggleEditForm(int cardId)
        {
            var card = specialCards.FirstOrDefault(c => c.Id == cardId);
            if (card == null) return;

            showEditForm = !showEditForm;
            if (showEditForm)
            {
                editingCardId = cardId;
                editCard = new CardModel
                {
                    CardType = card.CardType,
                    CardName = card.CardName,
                    NickName = card.NickName,
                    HizmetBinasiId = card.HizmetBinasiId,
                    CardNumber = card.CardNumber,
                    EnrollNumber = card.EnrollNumber,
                    Notes = card.Notes
                };
                showAddForm = false;
            }
        }

        private void OnNewCardNameChanged()
        {
            if (!string.IsNullOrWhiteSpace(newCard.CardName))
            {
                newCard.NickName = SGKPortalApp.Common.Helpers.StringHelper.GenerateNickName(newCard.CardName, 12);
            }
        }

        private void OnEditCardNameChanged()
        {
            if (!string.IsNullOrWhiteSpace(editCard.CardName))
            {
                editCard.NickName = SGKPortalApp.Common.Helpers.StringHelper.GenerateNickName(editCard.CardName, 12);
            }
        }

        private async Task CreateCard()
        {
            if (string.IsNullOrWhiteSpace(newCard.CardName))
            {
                await ToastService.ShowWarningAsync("Kart adı zorunludur");
                return;
            }

            if (newCard.CardNumber <= 0)
            {
                await ToastService.ShowWarningAsync("Geçerli bir kart numarası giriniz");
                return;
            }

            if (string.IsNullOrWhiteSpace(newCard.EnrollNumber))
            {
                await ToastService.ShowWarningAsync("EnrollNumber zorunludur");
                return;
            }

            try
            {
                var request = new SpecialCardCreateRequestDto
                {
                    CardType = newCard.CardType.Value,
                    CardName = newCard.CardName,
                    NickName = newCard.NickName,
                    HizmetBinasiId = newCard.HizmetBinasiId,
                    CardNumber = newCard.CardNumber,
                    EnrollNumber = newCard.EnrollNumber,
                    Notes = newCard.Notes
                };

                var result = await SpecialCardApiService.CreateAsync(request);
                if (result.Success)
                {
                    await ToastService.ShowSuccessAsync("Özel kart oluşturuldu");
                    showAddForm = false;
                    newCard = new CardModel { CardType = CardType.ViziteKarti };
                    await LoadCards();
                }
                else
                {
                    await ToastService.ShowErrorAsync(result.Message ?? "Oluşturma başarısız");
                }
            }
            catch (Exception ex)
            {
                await ToastService.ShowErrorAsync($"Hata: {ex.Message}");
            }
        }

        private async Task UpdateCard()
        {
            if (string.IsNullOrWhiteSpace(editCard.CardName))
            {
                await ToastService.ShowWarningAsync("Kart adı zorunludur");
                return;
            }

            if (editCard.CardNumber <= 0)
            {
                await ToastService.ShowWarningAsync("Geçerli bir kart numarası giriniz");
                return;
            }

            if (string.IsNullOrWhiteSpace(editCard.EnrollNumber))
            {
                await ToastService.ShowWarningAsync("EnrollNumber zorunludur");
                return;
            }

            try
            {
                var request = new SpecialCardUpdateRequestDto
                {
                    CardType = editCard.CardType.Value,
                    CardName = editCard.CardName,
                    NickName = editCard.NickName,
                    HizmetBinasiId = editCard.HizmetBinasiId,
                    CardNumber = editCard.CardNumber,
                    EnrollNumber = editCard.EnrollNumber,
                    Notes = editCard.Notes
                };

                var result = await SpecialCardApiService.UpdateAsync(editingCardId, request);
                if (result.Success)
                {
                    await ToastService.ShowSuccessAsync("Özel kart güncellendi");
                    showEditForm = false;
                    await LoadCards();
                }
                else
                {
                    await ToastService.ShowErrorAsync(result.Message ?? "Güncelleme başarısız");
                }
            }
            catch (Exception ex)
            {
                await ToastService.ShowErrorAsync($"Hata: {ex.Message}");
            }
        }

        private async Task DeleteCard(int id)
        {
            var card = specialCards.FirstOrDefault(c => c.Id == id);
            if (card == null) return;

            // TODO: Add confirmation dialog if needed
            try
            {
                var result = await SpecialCardApiService.DeleteAsync(id);
                if (result.Success)
                {
                    await ToastService.ShowSuccessAsync("Özel kart silindi");
                    await LoadCards();
                }
                else
                {
                    await ToastService.ShowErrorAsync(result.Message ?? "Silme başarısız");
                }
            }
            catch (Exception ex)
            {
                await ToastService.ShowErrorAsync($"Hata: {ex.Message}");
            }
        }

        private void OpenDeviceSelectModal(int cardId)
        {
            selectedCardForSending = cardId;
            selectedDeviceId = 0;
            showDeviceSelectModal = true;
        }

        private void CloseDeviceSelectModal()
        {
            showDeviceSelectModal = false;
            selectedCardForSending = 0;
            selectedDeviceId = 0;
        }

        private async Task SendCardToDevice()
        {
            if (selectedDeviceId == 0)
            {
                await ToastService.ShowWarningAsync("Lütfen bir cihaz seçin");
                return;
            }

            var card = specialCards.FirstOrDefault(c => c.Id == selectedCardForSending);
            if (card == null) return;

            var device = devices.FirstOrDefault(d => d.DeviceId == selectedDeviceId);
            if (device == null) return;

            try
            {
                await ToastService.ShowInfoAsync($"Kart cihaza gönderiliyor: {card.CardName} → {device.DeviceName}");

                var result = await SpecialCardApiService.SendCardToDeviceAsync(selectedCardForSending, selectedDeviceId);

                if (result.Success && result.Data != null)
                {
                    var syncResult = result.Data;
                    if (syncResult.IsSuccess)
                    {
                        await ToastService.ShowSuccessAsync($"✅ {syncResult.Message}");
                    }
                    else
                    {
                        await ToastService.ShowWarningAsync($"⚠️ Kart gönderilemedi: {syncResult.Details.FirstOrDefault()?.ErrorMessage ?? "Bilinmeyen hata"}");
                    }
                }
                else
                {
                    await ToastService.ShowErrorAsync(result.Message ?? "Kart cihaza gönderilemedi");
                }

                CloseDeviceSelectModal();
            }
            catch (Exception ex)
            {
                await ToastService.ShowErrorAsync($"Hata: {ex.Message}");
            }
        }

        private string GetCardTypeBadgeClass(CardType cardType)
        {
            return cardType switch
            {
                CardType.ViziteKarti => "bg-warning",
                CardType.SaatlikIzinKarti => "bg-success",
                CardType.GorevKarti => "bg-primary",
                CardType.MisafirKarti => "bg-info",
                _ => "bg-secondary"
            };
        }

        private string GetCardTypeText(CardType cardType)
        {
            return cardType switch
            {
                CardType.ViziteKarti => "Vizite Kartı",
                CardType.SaatlikIzinKarti => "Saatlik İzin Kartı",
                CardType.GorevKarti => "Görev Kartı",
                CardType.MisafirKarti => "Misafir Kartı",
                _ => "Bilinmiyor"
            };
        }

        // ═══════════════════════════════════════════════════════
        // EXPORT METHODS
        // ═══════════════════════════════════════════════════════

        private async Task ExportToExcel()
        {
            IsExporting = true;
            ExportType = "excel";

            try
            {
                using var workbook = new XLWorkbook();
                var worksheet = workbook.Worksheets.Add("Özel Kartlar");

                // Header
                worksheet.Cell(1, 1).Value = "Kart Adı";
                worksheet.Cell(1, 2).Value = "Kart Tipi";
                worksheet.Cell(1, 3).Value = "NickName";
                worksheet.Cell(1, 4).Value = "Hizmet Binası";
                worksheet.Cell(1, 5).Value = "Kart Numarası";
                worksheet.Cell(1, 6).Value = "EnrollNumber";
                worksheet.Cell(1, 7).Value = "Notlar";

                // Style header
                var headerRange = worksheet.Range(1, 1, 1, 7);
                headerRange.Style.Font.Bold = true;
                headerRange.Style.Fill.BackgroundColor = XLColor.LightBlue;

                // Data
                int row = 2;
                foreach (var card in filteredCards)
                {
                    worksheet.Cell(row, 1).Value = card.CardName;
                    worksheet.Cell(row, 2).Value = GetCardTypeText(card.CardType);
                    worksheet.Cell(row, 3).Value = card.NickName;
                    worksheet.Cell(row, 4).Value = card.HizmetBinasiAdi ?? "-";
                    worksheet.Cell(row, 5).Value = card.CardNumber.ToString();
                    worksheet.Cell(row, 6).Value = card.EnrollNumber;
                    worksheet.Cell(row, 7).Value = card.Notes ?? "-";
                    row++;
                }

                worksheet.Columns().AdjustToContents();

                using var stream = new MemoryStream();
                workbook.SaveAs(stream);
                var content = stream.ToArray();
                var fileName = $"OzelKartlar_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";

                await JSRuntime.InvokeVoidAsync("downloadFile", fileName, Convert.ToBase64String(content), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
                await ToastService.ShowSuccessAsync("Excel dosyası başarıyla indirildi!");
            }
            catch (Exception ex)
            {
                await ToastService.ShowErrorAsync($"Excel oluşturulurken hata: {ex.Message}");
            }
            finally
            {
                IsExporting = false;
                ExportType = string.Empty;
            }
        }

        private async Task ExportToPdf()
        {
            IsExporting = true;
            ExportType = "pdf";

            try
            {
                var document = Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        page.Size(PageSizes.A4.Landscape());
                        page.Margin(2, Unit.Centimetre);
                        page.DefaultTextStyle(x => x.FontSize(9));

                        page.Header().Text("Özel Kartlar Listesi").FontSize(18).Bold();

                        page.Content().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(3);
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(3);
                            });

                            table.Header(header =>
                            {
                                header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Kart Adı").Bold();
                                header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Kart Tipi").Bold();
                                header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("NickName").Bold();
                                header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Hizmet Binası").Bold();
                                header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Kart No").Bold();
                                header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Enroll No").Bold();
                                header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Notlar").Bold();
                            });

                            foreach (var card in filteredCards)
                            {
                                table.Cell().Padding(3).Text(card.CardName);
                                table.Cell().Padding(3).Text(GetCardTypeText(card.CardType));
                                table.Cell().Padding(3).Text(card.NickName);
                                table.Cell().Padding(3).Text(card.HizmetBinasiAdi ?? "-");
                                table.Cell().Padding(3).Text(card.CardNumber.ToString());
                                table.Cell().Padding(3).Text(card.EnrollNumber);
                                table.Cell().Padding(3).Text(card.Notes ?? "-");
                            }
                        });

                        page.Footer().AlignCenter().Text(text =>
                        {
                            text.Span("Sayfa ");
                            text.CurrentPageNumber();
                            text.Span(" / ");
                            text.TotalPages();
                        });
                    });
                });

                var pdfBytes = document.GeneratePdf();
                var fileName = $"OzelKartlar_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";

                await JSRuntime.InvokeVoidAsync("downloadFile", fileName, Convert.ToBase64String(pdfBytes), "application/pdf");
                await ToastService.ShowSuccessAsync("PDF dosyası başarıyla indirildi!");
            }
            catch (Exception ex)
            {
                await ToastService.ShowErrorAsync($"PDF oluşturulurken hata: {ex.Message}");
            }
            finally
            {
                IsExporting = false;
                ExportType = string.Empty;
            }
        }

        private class CardModel
        {
            public CardType? CardType { get; set; }
            public string CardName { get; set; } = string.Empty;
            public string NickName { get; set; } = string.Empty;
            public int? HizmetBinasiId { get; set; }
            public long CardNumber { get; set; }
            public string EnrollNumber { get; set; } = string.Empty;
            public string? Notes { get; set; }
        }
    }
}
