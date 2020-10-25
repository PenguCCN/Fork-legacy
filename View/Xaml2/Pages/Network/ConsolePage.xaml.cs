﻿using System;
using System.Collections.Specialized;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Documents;
using Fork.Logic;
using Fork.Logic.Model.ServerConsole;
using Fork.ViewModel;

namespace Fork.View.Xaml2.Pages.Network
{
    public partial class ConsolePage : Page
    {
        private NetworkViewModel viewModel;
        
        public ConsolePage(NetworkViewModel viewModel)
        {
            InitializeComponent();
            this.viewModel = viewModel;
            DataContext = this.viewModel;

            viewModel.ConsoleOutList.CollectionChanged += UpdateConsoleOut;
        }
        

        
        #region autoscrolling
        /// <summary>
        /// Automatically scrolls the scrollviewer
        /// </summary>

        private bool AutoScroll = true;

        private void ScrollViewer_ScrollChanged(Object sender, ScrollChangedEventArgs e)
        {
            ScrollViewer scrollViewer = sender as ScrollViewer;
            // User scroll event : set or unset auto-scroll mode
            if (e.ExtentHeightChange == 0)
            {   // Content unchanged : uSelectPlayerList
                if (scrollViewer.VerticalOffset == scrollViewer.ScrollableHeight)
                {   // Scroll bar is in bottom
                    // Set auto-scroll mode
                    AutoScroll = true;
                }
                else
                {   // Scroll bar isn't in bottom
                    // Unset auto-scroll mode
                    AutoScroll = false;
                }
            }

            // Content scroll event : auto-scroll eventually
            if (AutoScroll && e.ExtentHeightChange != 0)
            {   // Content changed and auto-scroll mode set
                // Autoscroll
                scrollViewer.ScrollToVerticalOffset(scrollViewer.ExtentHeight);
            }
        }
        #endregion autoscrolling

        private void UpdateConsoleOut(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add && e.NewItems != null)
            {
                foreach (var newItem in e.NewItems)
                {
                    if (newItem is ConsoleMessage newConsoleMessage)
                    {
                        if (e.NewStartingIndex*2 < ConsoleParagraph.Inlines.Count)
                        {
                            var elementAfter = ConsoleParagraph.Inlines.ElementAt(e.NewStartingIndex*2);
                            ConsoleParagraph.Inlines.InsertBefore(elementAfter,
                                new Run{Text = newConsoleMessage.Content, Foreground = newConsoleMessage.Level.Color()});
                            ConsoleParagraph.Inlines.InsertBefore(elementAfter, new LineBreak());
                        }
                        else
                        {
                            ConsoleParagraph.Inlines.Add(
                                new Run{Text = newConsoleMessage.Content, Foreground = newConsoleMessage.Level.Color()});
                            ConsoleParagraph.Inlines.Add(new LineBreak());
                        }
                    }
                } 
            }
            
            if (e.Action == NotifyCollectionChangedAction.Remove && e.OldItems != null)
            {
                int amountToRemove = e.OldItems.Count * 2;
                int index = e.OldStartingIndex * 2;
                for (int i = 0; i < amountToRemove; i++)
                {
                    ConsoleParagraph.Inlines.Remove(ConsoleParagraph.Inlines.ElementAt(index));
                }
            }
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string query = SearchBox.Text;
            viewModel.ApplySearchQueryToConsole(query);
        }
    }
}
