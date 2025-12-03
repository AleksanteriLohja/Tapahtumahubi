using System;
using Microsoft.Maui.Controls;
using Tapahtumahubi.App.ViewModels;

namespace Tapahtumahubi.App
{
    // Vastaanottaa "EventId" -parametrin muokkaustilaa varten
    public partial class NewEventPage : ContentPage, IQueryAttributable
    {
        private NewEventPageViewModel _vm = null!;

        public NewEventPage()
        {
            InitializeComponent();

            // Ota VM DI:stä ja aseta BindingContext
            _vm = ServiceHelper.GetRequiredService<NewEventPageViewModel>();
            BindingContext = _vm;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await _vm.InitializeAsync(); // täyttää kentät muokkaustilassa
        }

        // Välilehdeltä tai listasta tullessa: "EventId" annetaan query-parametrina
        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query.TryGetValue("EventId", out var idObj) && idObj is int id)
            {
                _vm.SetEditingId(id);
            }
        }

        // Alla säilytetään aiempi kenttäfokuslogiikka
        private void OnTitleCompleted(object? sender, EventArgs e) => LocationEntry?.Focus();
        private void OnLocationCompleted(object? sender, EventArgs e) => DatePickerControl?.Focus();
        private void OnMaxParticipantsCompleted(object? sender, EventArgs e) => SaveButton?.Focus();
    }
}