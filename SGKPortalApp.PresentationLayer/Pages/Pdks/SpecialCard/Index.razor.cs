using Microsoft.AspNetCore.Components;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.PdksIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.PdksIslemleri;
using SGKPortalApp.BusinessObjectLayer.Enums.PdksIslemleri;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.ZKTeco;
using SGKPortalApp.PresentationLayer.Services.UIServices.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SGKPortalApp.PresentationLayer.Pages.Pdks.SpecialCard
{
    public partial class Index
    {
        [Inject] private ISpecialCardApiService SpecialCardApiService { get; set; } = default!;
        [Inject] private IToastService ToastService { get; set; } = default!;

        private List<SpecialCardResponseDto> specialCards = new();
        private List<SpecialCardResponseDto> filteredCards = new();
        private List<SpecialCardResponseDto> pagedCards = new();
        
        private bool isLoading = true;
        private bool showAddForm = false;
        private bool showEditForm = false;
        
        private CardModel newCard = new() { CardType = CardType.ViziteKarti };
        private CardModel editCard = new();
        private int editingCardId = 0;

        // Filtering
        private string searchTerm = string.Empty;
        private string selectedCardType = string.Empty;

        // Pagination
        private int currentPage = 1;
        private int pageSize = 25;
        private int totalPages => (int)Math.Ceiling((double)filteredCards.Count / pageSize);

        protected override async Task OnInitializedAsync()
        {
            await LoadCards();
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

        private async Task SendCardToAllDevices(int cardId)
        {
            var card = specialCards.FirstOrDefault(c => c.Id == cardId);
            if (card == null) return;

            try
            {
                await ToastService.ShowInfoAsync($"Kart tüm cihazlara gönderiliyor: {card.CardName}");
                
                var result = await SpecialCardApiService.SendCardToAllDevicesAsync(cardId);
                
                if (result.Success && result.Data != null)
                {
                    var syncResult = result.Data;
                    if (syncResult.IsSuccess)
                    {
                        await ToastService.ShowSuccessAsync($"✅ {syncResult.Message}");
                    }
                    else
                    {
                        await ToastService.ShowWarningAsync($"⚠️ Kısmi başarı: {syncResult.SuccessCount}/{syncResult.TotalDevices} cihaza gönderildi");
                    }
                }
                else
                {
                    await ToastService.ShowErrorAsync(result.Message ?? "Kart cihazlara gönderilemedi");
                }
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

        private class CardModel
        {
            public CardType? CardType { get; set; }
            public string CardName { get; set; } = string.Empty;
            public string NickName { get; set; } = string.Empty;
            public long CardNumber { get; set; }
            public string EnrollNumber { get; set; } = string.Empty;
            public string? Notes { get; set; }
        }
    }
}
