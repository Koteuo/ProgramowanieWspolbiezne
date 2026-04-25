//__________________________________________________________________________________________
//
//  Copyright 2024 Mariusz Postol LODZ POLAND.
//
//  To be in touch join the community by pressing the `Watch` button and to get started
//  comment using the discussion panel at
//  https://github.com/mpostol/TP/discussions/182
//__________________________________________________________________________________________

using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using TP.ConcurrentProgramming.Presentation.Model;
using TP.ConcurrentProgramming.Presentation.ViewModel.MVVMLight;
using ModelIBall = TP.ConcurrentProgramming.Presentation.Model.IBall;

namespace TP.ConcurrentProgramming.Presentation.ViewModel
{
    public class MainWindowViewModel : ViewModelBase, IDisposable
    {

        public MainWindowViewModel() : this(null)
        { }

        private int _ballsNumber;
        public int BallsNumber
        {
            get => _ballsNumber;
            set
            {
                _ballsNumber = value;
                RaisePropertyChanged();
            }
        }

        public ICommand StartCommand { get; }
        public ICommand StopCommand { get; }

        public MainWindowViewModel(ModelAbstractApi modelLayerAPI)
        {
            ModelLayer = modelLayerAPI == null ? ModelAbstractApi.CreateModel() : modelLayerAPI;
            // Subskrypcja powinna trwać przez cały czas działania, aby odbierać nowe kulki
            Observer = ModelLayer.Subscribe<ModelIBall>(x => Balls.Add(x));

            StartCommand = new RelayCommand(() => Start(BallsNumber), () => !IsRunning);
            StopCommand = new RelayCommand(Stop, () => IsRunning);
        }

        private bool _isRunning;
        public bool IsRunning
        {
            get => _isRunning;
            set { _isRunning = value; RaisePropertyChanged(); ((RelayCommand)StartCommand).RaiseCanExecuteChanged(); ((RelayCommand)StopCommand).RaiseCanExecuteChanged(); }
        }

        public void Start(int numberOfBalls)
        {
            if (numberOfBalls <= 0) return;
            IsRunning = true;
            ModelLayer.Start(numberOfBalls);
        }

        public void Stop()
        {
            IsRunning = false;
            ModelLayer.Stop();
            Balls.Clear();
        }

        public ObservableCollection<ModelIBall> Balls { get; } = new ObservableCollection<ModelIBall>();

        protected virtual void Dispose(bool disposing)
        {
            if (!Disposed)
            {
                if (disposing)
                {
                    Balls.Clear();
                    Observer.Dispose();
                    ModelLayer.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                Disposed = true;
            }
        }

        public void Dispose()
        {
            if (Disposed)
                throw new ObjectDisposedException(nameof(MainWindowViewModel));
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        private IDisposable Observer = null;
        private ModelAbstractApi ModelLayer;
        private bool Disposed = false;
    }
}