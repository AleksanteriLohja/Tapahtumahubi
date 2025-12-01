using System;
using Microsoft.Maui.Controls;

namespace Tapahtumahubi.App
{
    public partial class NewEventPage : ContentPage
    {
        public NewEventPage()
        {
            InitializeComponent();
        }

        // Enter Title -> Location
        private void OnTitleCompleted(object? sender, EventArgs e)
        {
            LocationEntry?.Focus();
        }

        // Enter Location -> Date
        private void OnLocationCompleted(object? sender, EventArgs e)
        {
            DatePickerControl?.Focus();
        }

        // Enter MaxParticipants -> Save
        private void OnMaxParticipantsCompleted(object? sender, EventArgs e)
        {
            SaveButton?.Focus();
        }
    }
}