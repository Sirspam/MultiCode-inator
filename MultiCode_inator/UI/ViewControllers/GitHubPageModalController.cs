﻿using System.ComponentModel;
using System.Reflection;
using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Parser;
using HMUI;
using UnityEngine;
using Component = UnityEngine.Component;

namespace MultiCode_inator.UI.ViewControllers
{
	// Designed so I can be lazy and copy this across projects
	internal class GitHubPageModalController : INotifyPropertyChanged
	{
		private bool _parsed;
		private string _modalText = null!;
		private string _githubPath = null!;
		
		public event PropertyChangedEventHandler? PropertyChanged;

		[UIComponent("modal")] private readonly ModalView _modalView = null!;

		[UIParams] private readonly BSMLParserParams _parserParams = null!;

		[UIValue("modal-text")]
		private string ModalText
		{
			get => _modalText;
			set
			{
				_modalText = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ModalText)));
			}
		}

		private void Parse(Component parentTransform, string pluginName, string githubPath)
		{
			if (!_parsed)
			{
				var executingAssembly = Assembly.GetExecutingAssembly();
				BSMLParser.Instance.Parse(Utilities.GetResourceContent(executingAssembly, $"{executingAssembly.GetName().Name}.UI.Views.GitHubPageModalView.bsml"), parentTransform.gameObject, this);
				ModalText = $"Open {pluginName}'s GitHub Page?"; // Might be a bit silly to set the text like this but it felt odd having the version text get everything from the manifest while the modal was static
				_githubPath = githubPath;
				_modalView.name = $"{pluginName}GitHubModal";				
				_parsed = true;
			}
		}

		public void ShowModal(Transform parentTransform, string pluginName,string githubPath)
		{
			Parse(parentTransform, pluginName, githubPath);

			_parserParams.EmitEvent("close-modal");
			_parserParams.EmitEvent("open-modal");
		}

		[UIAction("yes-clicked")]
		private void YesClicked()
		{
			Application.OpenURL(_githubPath);
			_parserParams.EmitEvent("close-modal");
		}

		[UIAction("no-clicked")]
		private void NoClicked()
		{
			_parserParams.EmitEvent("close-modal");
		}
	}
}