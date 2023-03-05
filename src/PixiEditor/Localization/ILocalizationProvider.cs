﻿using System.IO;

namespace PixiEditor.Localization;

public interface ILocalizationProvider
{
    public static ILocalizationProvider Current => ViewModelMain.Current.LocalizationProvider;
    
    public string LocalizationDataPath { get; }
    public LocalizationData LocalizationData { get; }
    public Language CurrentLanguage { get; set; }
    public event Action<Language> OnLanguageChanged;

    /// <summary>
    ///     Loads the localization data from the specified file.
    /// </summary>
    public void LoadData();
    public void LoadLanguage(LanguageData languageData);
    public Language DefaultLanguage { get; }
}
