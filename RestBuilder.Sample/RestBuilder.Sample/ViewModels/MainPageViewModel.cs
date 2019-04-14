using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;
using RestBuilder.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestBuilder.Sample.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        private readonly IPageDialogService _dialogService;
        private readonly IBinRestService _binRestService;
        private const string Unassigned = "Unassigned";

        public DelegateCommand CreateBinCommand { get; }
        public DelegateCommand GetBinCommand { get; }
        public DelegateCommand GetBinWithDefaultCommand { get; }
        public DelegateCommand DeleteBinCommand { get; }
        public DelegateCommand PostUserCommand { get; }
        public DelegateCommand PostUserWithExceptionCommand { get; }
        public DelegateCommand GetUserCommand { get; }
        public DelegateCommand GetUser2Command { get; }

        public MainPageViewModel(INavigationService navigationService, IPageDialogService dialogService, IBinRestService binRestService)
            : base(navigationService)
        {
            _dialogService = dialogService;
            _binRestService = binRestService;

            Title = "RestBuilder Sample";

            CreateBinCommand = new DelegateCommand(async () => await OnCreateBinCommandExecuted());
            GetBinCommand = new DelegateCommand(async () => await OnGetBinCommandExecuted());
            GetBinWithDefaultCommand = new DelegateCommand(async () => await OnGetBinWithDefaultCommandExecuted());
            DeleteBinCommand = new DelegateCommand(async () => await OnDeleteBinCommandExecuted());
            PostUserCommand = new DelegateCommand(async () => await OnPostUserCommandExecuted());
            PostUserWithExceptionCommand = new DelegateCommand(async () => await OnPostUserCommandWithExceptionExecuted());
            GetUserCommand = new DelegateCommand(async () => await OnGetUserCommandExecuted());
            GetUser2Command = new DelegateCommand(async () => await OnGetUser2CommandExecuted());

            PostBin = null;
        }

        private async Task OnCreateBinCommandExecuted()
        {
            PostBin = await _binRestService.CreateBin();
        }

        private async Task OnGetBinCommandExecuted()
        {
            PostBin = await _binRestService.GetBin(BinId);
        }

        private async Task OnGetBinWithDefaultCommandExecuted()
        {
            var defaultPostBin = new PostBin { binId = "MyDefault" };

            PostBin = await _binRestService.GetBin(BinId, defaultPostBin);
        }

        private async Task OnDeleteBinCommandExecuted()
        {
            var deleted = await _binRestService.DeleteBin(BinId);

            if( deleted)
            {
                PostBin = null;
                UserRequestId = null;
            }
        }

        private async Task OnPostUserCommandExecuted()
        {
            //if (PostBin == null)
                //return;

            var user = new User
            {
                UserId = Guid.NewGuid(),
                Login = "newUser123",
                Password = "mysecret",
                Email = "Tester@testy.com",
                CreatedDate = DateTime.UtcNow,
                Name = "Testy Tester"
            };

            UserRequestId = await _binRestService.PostUser(BinId, user);
        }

        private async Task OnPostUserCommandWithExceptionExecuted()
        {
            var user = new User
            {
                UserId = Guid.NewGuid(),
                Login = "exception123",
                Password = "mysecret",
                Email = "Tester@testy.com",
                CreatedDate = DateTime.UtcNow,
                Name = "Exception Tester"
            };

            UserRequestId = await _binRestService.PostUserWithException(BinId, user);
        }


        private async Task OnGetUserCommandExecuted()
        {
            var user = await _binRestService.GetUser(BinId, UserRequestId);

            if (user == null)
                await _dialogService.DisplayAlertAsync("Alert1", "User object is null", "ok");
            else
                await _dialogService.DisplayAlertAsync("Alert1", $"User Login is {user.Login}", "ok");
        }

        private async Task OnGetUser2CommandExecuted()
        {
            var user = await _binRestService.GetUser2(BinId, UserRequestId);

            if (user == null)
                await _dialogService.DisplayAlertAsync("Alert2", "User object is null", "ok");
            else
                await _dialogService.DisplayAlertAsync("Alert2", $"User Login is {user.Login}", "ok");
        }


        private string _userRequestId;
        public string UserRequestId
        {
            get { return _userRequestId; }
            set { SetProperty(ref _userRequestId, value); }
        }

        public string BinId => PostBin?.binId;

        public string EndPoint => (PostBin == null) ? null : $"https://postb.in/{PostBin.binId}";

        private PostBin _postBin;
        public PostBin PostBin
        {
            get { return _postBin; }
            set
            {
                SetProperty(ref _postBin, value);
                RaisePropertyChanged(nameof(BinId));
                RaisePropertyChanged(nameof(EndPoint));
            }
        }
    }
}
