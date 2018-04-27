/*
 * CommitFormatter - http://github.com/kria/CommitFormatter
 * 
 * Copyright (C) 2015 Kristian Adrup
 * 
 * This file is part of CommitFormatter.
 * 
 * CommitFormatter is free software: you can redistribute it and/or modify 
 * it under the terms of the GNU General Public License as published by 
 * the Free Software Foundation, either version 3 of the License, or (at 
 * your option) any later version. See included file COPYING for details.
 */

using Adrup.CommitFormatter.Core;
using Microsoft.TeamExplorerSample;
using Microsoft.TeamFoundation.Controls;
using Microsoft.TeamFoundation.Controls.WPF;
using Microsoft.TeamFoundation.Controls.WPF.TeamExplorer.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace Adrup.CommitFormatter.TeamFoundation
{
    [TeamExplorerSection(FormatterSection.SectionId, TeamExplorerPageIds.GitChanges, 900)]
    public class FormatterSection : TeamExplorerBaseSection
    {
        public const string SectionId = "18af7740-585c-4410-8d77-500f70061f6e";

        private int _subjectWidth;
        private int _bodyWidth;
        private int _fontSize;
        private bool _useMonospacedFont;
        private bool _blankSecondLine;
        private bool _enableSpellCheck;

        private TextBox _commitMessageBox = null;
        private LabeledTextBox _labeledTextBox = null;
        private bool _isCurrentlyChangingText = false;
        private CharsLeftAdorner _adorner;

        public FormatterSection() : base()
        {
            IsVisible = false;
        }

        public override void Initialize(object sender, SectionInitializeEventArgs e)
        {
            base.Initialize(sender, e);

            var settings = new FormatterSettings(ServiceProvider);
            _subjectWidth = settings.SubjectWidth;
            _bodyWidth = settings.BodyWidth;
            _fontSize = settings.FontSize;
            _useMonospacedFont = settings.UseMonospacedFont;
            _blankSecondLine = settings.BlankSecondLine;
            _enableSpellCheck = settings.EnableSpellCheck;
        }

        public override void Loaded(object sender, SectionLoadedEventArgs e)
        {
            var service = GetService<TeamExplorerViewModel>();
            if (service == null)
            {
                ShowNotification("Commit Formatter: Can't get TeamExplorerViewModel", NotificationType.Error);
                return;
            }

            var changesGuid = Guid.Parse(TeamExplorerPageIds.GitChanges);

            var pages = new List<ITeamExplorerPage>();
            if (service.CurrentPage != null)
                pages.Add(service.CurrentPage);
            pages.AddRange(service.UndockedPages);

            var changesPage = pages.FirstOrDefault(p => p.GetId() == changesGuid); 
            if (changesPage == null)
            {
                ShowNotification("Commit Formatter: Can't get the Changes page", NotificationType.Error);
                return;
            }

            var view = changesPage.PageContent as UserControl;
            if (view == null)
            {
                ShowNotification("Commit Formatter: Can't get the Changes view", NotificationType.Error);
                return;
            }

            _labeledTextBox = view.FindName("commentTextBox") as LabeledTextBox;
            if (_labeledTextBox == null)
            {
                ShowNotification("Commit Formatter: Can't find commentTextBox", NotificationType.Error);
                return;
            }
            
            _commitMessageBox = _labeledTextBox.FindName("textBox") as TextBox;
            if (_commitMessageBox == null)
            {
                ShowNotification("Commit Formatter: Can't find textBox", NotificationType.Error);
                return;
            }

            _commitMessageBox.TextChanged += OnCommitMessageChanged;
            _commitMessageBox.SelectionChanged += OnSelectionChanged;
            _commitMessageBox.SpellCheck.IsEnabled = _enableSpellCheck;

            if (_useMonospacedFont)
                _commitMessageBox.FontFamily = new FontFamily("Consolas");
            _commitMessageBox.FontSize = _fontSize;
            _commitMessageBox.TextWrapping = TextWrapping.NoWrap;
        }

        private bool InitializeAdorner()
        {
            if (_adorner == null)
            {
                var myAdornerLayer = AdornerLayer.GetAdornerLayer(_labeledTextBox);
                if (myAdornerLayer == null) return false;

                _adorner = new CharsLeftAdorner(_labeledTextBox);
                myAdornerLayer.Add(_adorner);
            }

            return true;
        }

        void OnSelectionChanged(object sender, RoutedEventArgs e)
        {
            if (InitializeAdorner())
            {
                int charsLeft = TextHelper.CountLineCharsLeft(_commitMessageBox.Text, _commitMessageBox.CaretIndex, _subjectWidth, _bodyWidth, _blankSecondLine);
                _adorner.DataContext = charsLeft;
                _adorner.InvalidateVisual();
            }
        }

        private void OnCommitMessageChanged(object sender, TextChangedEventArgs e)
        {
            if (_isCurrentlyChangingText) return;
            var text = _commitMessageBox.Text;
            int caretIndexDelta = 0;
            int caretIndex = _commitMessageBox.CaretIndex;
            
            string newtext = TextHelper.Wrap(text, caretIndex, _subjectWidth, _bodyWidth, _blankSecondLine, out caretIndexDelta);
            if (newtext != _commitMessageBox.Text)
            {
                _isCurrentlyChangingText = true;
                _commitMessageBox.Text = newtext;

                _commitMessageBox.CaretIndex = caretIndex + caretIndexDelta;
                _isCurrentlyChangingText = false;
            }
        }

        public override void Dispose()
        {
            base.Dispose();

            if (_commitMessageBox != null)
            {
                _commitMessageBox.TextChanged -= OnCommitMessageChanged;
                _commitMessageBox.SelectionChanged -= OnSelectionChanged;
            }
        }

    }

    public class CharsLeftAdorner : Adorner
    {
        public CharsLeftAdorner(UIElement adornedElement)
            : base(adornedElement)
        {
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            Rect adornedElementRect = new Rect(this.AdornedElement.RenderSize);

            int charsLeft = 0;
            int.TryParse(DataContext.ToString(), out charsLeft);

            SolidColorBrush renderBrush = new SolidColorBrush(charsLeft < 0 ? Colors.Red : Colors.Green);
            renderBrush.Opacity = 0.6;

            var typeface = new Typeface("Consolas");
            
            var formattedText = new FormattedText(this.DataContext.ToString(), CultureInfo.CurrentCulture, System.Windows.FlowDirection.LeftToRight, typeface, 11, renderBrush);
            formattedText.TextAlignment = TextAlignment.Right;
            var anchor = adornedElementRect.TopRight;
            anchor.Offset(0, -formattedText.Height);

            drawingContext.DrawText(formattedText, anchor);
        }
    }
}
