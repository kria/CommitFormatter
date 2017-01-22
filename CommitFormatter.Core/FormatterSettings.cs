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

using Microsoft.VisualStudio.Settings;
using Microsoft.VisualStudio.Shell.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adrup.CommitFormatter.Core
{
    public class FormatterSettings
    {
        public const string CollectionPath = "Adrup.CommitFormatter";
        public const string SubjectWidthKey = "SubjectWidth";
        public const string BodyWidthKey = "BodyWidth";
        public const string FontSizeKey = "FontSize";
        public const string UseMonospacedFontKey = "UseMonospacedFont";
        public const string BlankSecondLineKey = "BlankSecondLine";

        private WritableSettingsStore _userSettingsStore;

        public FormatterSettings(IServiceProvider serviceProvider)
        {
            SettingsManager settingsManager = new ShellSettingsManager(serviceProvider);
            _userSettingsStore = settingsManager.GetWritableSettingsStore(SettingsScope.UserSettings);
            if (_userSettingsStore != null && !_userSettingsStore.CollectionExists(CollectionPath))
            {
                _userSettingsStore.CreateCollection(CollectionPath);
                SubjectWidth = 50;
                BodyWidth = 72;
                FontSize = 11;
                UseMonospacedFont = true;
                BlankSecondLine = true;
            }
        }

        public int SubjectWidth
        {
            get { return _userSettingsStore.GetInt32(CollectionPath, SubjectWidthKey); }
            set { _userSettingsStore.SetInt32(CollectionPath, SubjectWidthKey, value); }
        }

        public int BodyWidth
        {
            get { return _userSettingsStore.GetInt32(CollectionPath, BodyWidthKey); }
            set { _userSettingsStore.SetInt32(CollectionPath, BodyWidthKey, value); }
        }

        public int FontSize
        {
            get { return _userSettingsStore.GetInt32(CollectionPath, FontSizeKey); }
            set { _userSettingsStore.SetInt32(CollectionPath, FontSizeKey, value); }
        }

        public bool UseMonospacedFont
        {
            get { return _userSettingsStore.GetBoolean(CollectionPath, UseMonospacedFontKey, true); }
            set { _userSettingsStore.SetBoolean(CollectionPath, UseMonospacedFontKey, value); }
        }

        public bool BlankSecondLine
        {
            get { return _userSettingsStore.GetBoolean(CollectionPath, BlankSecondLineKey, true); }
            set { _userSettingsStore.SetBoolean(CollectionPath, BlankSecondLineKey, value); }
        }
    }
}
