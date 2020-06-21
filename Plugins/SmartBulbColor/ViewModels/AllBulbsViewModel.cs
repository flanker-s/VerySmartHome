﻿using SmartBulbColor.PluginApp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Windows.Input;
using System.Windows.Media;

namespace SmartBulbColor.ViewModels
{
    internal class AllBulbsViewModel : ViewModelBase
    {
        SynchronizationContext Context = SynchronizationContext.Current;

        private AppMediator Mediator;

        List<string> _groupNames;
        public List<string> GroupNames
        {
            get => Mediator.GetGroupNames();
            set
            {
                _groupNames = value;
                OnPropertyChanged("GroupNames");
            }
        }
        string _selectedGroupName;
        public string SelectedGroupName
        {
            get => _selectedGroupName;
            set
            {
                _selectedGroupName = value;
                OnPropertyChanged("SelectedGroupName");
            }
        }

        public ObservableCollection<ColorBulbViewModel> ColorBulbVMs { get; set; }

        List<ColorBulbViewModel> _selectedBulbVMs;
        public List<ColorBulbViewModel> SelectedBulbVMs
        {
            get { return _selectedBulbVMs; }
            set
            {
                _selectedBulbVMs = value;
                OnPropertyChanged("SelectedBulbVMs");
            }
        }
        private SolidColorBrush _pickerBrush = Brushes.White;
        public SolidColorBrush PickerBrush
        {
            get { return _pickerBrush; }
            set
            {
                _pickerBrush = value;
                SetColorWithBrush(value);
            }
        }

        public AllBulbsViewModel(string groupName, AppMediator mediator)
        {
            ColorBulbVMs = new ObservableCollection<ColorBulbViewModel>();
            SelectedBulbVMs = new List<ColorBulbViewModel>();
            Mediator = mediator;
            Mediator.BulbAdded += (bulb) => OnNewBulbAdded(new ColorBulbViewModel(Mediator, bulb));
        }

        public ICommand TogglePower
        {
            get { return new ControllerCommand(ExecuteTogglePower, CanExecuteTogglePower); }
        }
        void ExecuteTogglePower(object parametr)
        {
            foreach (var bulbVM in SelectedBulbVMs)
            {
                if (bulbVM.TogglePower.CanExecute(parametr))
                    bulbVM.TogglePower.Execute(parametr);
            }
        }
        bool CanExecuteTogglePower(object parametr)
        {
            if (SelectedBulbVMs == null || SelectedBulbVMs.Count == 0)
                return false;
            else
                return true;
        }
        public ICommand SetNormalLight
        {
            get { return new ControllerCommand(ExecuteSetNormalLight, CanExecuteSetNormalLight); }
        }
        void ExecuteSetNormalLight(object parametr)
        {
            foreach (var bulbVM in SelectedBulbVMs)
            {
                if (bulbVM.TurnNormalLightON.CanExecute(parametr))
                    bulbVM.TurnNormalLightON.Execute(parametr);
            }
        }
        bool CanExecuteSetNormalLight(object parametr)
        {
            if (SelectedBulbVMs == null || SelectedBulbVMs.Count == 0)
                return false;
            else
                return true;
        }
        public ICommand ToggleAmbientLight
        {
            get { return new ControllerCommand(ExecuteToggleAmbientLight, CanExecuteToggleAmbientLight); }
        }
        void ExecuteToggleAmbientLight(object parametr)
        {
            foreach (var bulbVM in SelectedBulbVMs)
            {
                if (bulbVM.ToggleAmbientLight.CanExecute(parametr))
                    bulbVM.ToggleAmbientLight.Execute(parametr);
            }
        }
        bool CanExecuteToggleAmbientLight(object parametr)
        {
            if (SelectedBulbVMs == null || SelectedBulbVMs.Count == 0)
                return false;
            else
                return true;
        }
        public ICommand MoveToGroup
        {
            get { return new ControllerCommand(ExecuteMoveToGroup, CanExecuteMoveToGroup); }
        }
        void ExecuteMoveToGroup(object parametr)
        {

        }
        bool CanExecuteMoveToGroup(object parametr)
        {
            return GroupNames.Count != 0;
        }
        public void OnNewBulbAdded(ColorBulbViewModel bulbVM)
        {
            Context.Post((object state) => { ColorBulbVMs.Add(bulbVM); }, new object());  
        }
        public void OnGroupNamesChanged(string[] groupNames)
        {
            GroupNames = groupNames.ToList();
        }
        private void SetColorWithBrush(Object parametr)
        {
            if (SelectedBulbVMs != null && SelectedBulbVMs.Count != 0)
            {
                var brush = (SolidColorBrush)parametr;

                foreach (var bulbVM in SelectedBulbVMs)
                {
                    bulbVM.SetColor(brush);
                }
            }
        }
    }
}
