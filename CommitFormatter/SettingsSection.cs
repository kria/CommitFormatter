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

using Microsoft.TeamExplorerSample;
using Microsoft.TeamFoundation.Controls;
using Microsoft.VisualStudio.Settings;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Shell.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adrup.CommitFormatter
{
    [TeamExplorerSection(SettingsSection.SectionId, TeamExplorerPageIds.GitSettings, 900)]
    public class SettingsSection : TeamExplorerBaseSection
    {
        public const string SectionId = "0470ed5b-d583-4d5e-b2e7-3d35225f93c6";

        private FormatterSettings _settings;

        public SettingsSection() : base()
        {
            Title = "Commit Formatter Settings";
            SectionContent = new SettingsSectionView();
        }

        public override void Initialize(object sender, SectionInitializeEventArgs e)
        {
            base.Initialize(sender, e);

            _settings = new FormatterSettings(ServiceProvider);
            var view = (SettingsSectionView)SectionContent;
            view.txtSubjectWidth.Text = _settings.SubjectWidth.ToString();
            view.txtBodyWidth.Text = _settings.BodyWidth.ToString();
            view.txtFontSize.Text = _settings.FontSize.ToString();
            view.chkUseMonospacedFont.IsChecked = _settings.UseMonospacedFont;
            view.chkBlankSecondLine.IsChecked = _settings.BlankSecondLine;
        }

        public override void SaveContext(object sender, SectionSaveContextEventArgs e)
        {
            var view = SectionContent as SettingsSectionView;

            int value;
            if (int.TryParse(view.txtSubjectWidth.Text, out value)) _settings.SubjectWidth = value;
            if (int.TryParse(view.txtBodyWidth.Text, out value)) _settings.BodyWidth = value;
            if (int.TryParse(view.txtFontSize.Text, out value)) _settings.FontSize = value;
            _settings.UseMonospacedFont = view.chkUseMonospacedFont.IsChecked.Value;
            _settings.BlankSecondLine = view.chkBlankSecondLine.IsChecked.Value;
        }

    }
}
