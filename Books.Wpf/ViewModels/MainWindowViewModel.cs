using Books.Core.DataTransferObjects;
using Books.Core.Entities;
using Books.Persistence;
using Books.Wpf.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;

namespace Books.Wpf.ViewModels
{
    public class MainWindowViewModel : BaseViewModel
    {
        private ObservableCollection<BookDto> _books;
        private BookDto _selectedBook;
        private string _filterText;
        private CollectionView _view;

        public BookDto SelectedBook 
        {
            get => _selectedBook;
            set
            {
                _selectedBook = value;
                OnPropertyChanged();
            }
        }
        public string FilterText 
        {
            get => _filterText;
            set
            {
                _filterText = value;
                _view.Refresh();
                OnPropertyChanged();
            }
        }
        public bool BookFilter(object item)
        {
            if (String.IsNullOrEmpty(FilterText))
            {
                return true;
            }
            else
            {
                return ((item as BookDto).Title.Contains(FilterText, StringComparison.OrdinalIgnoreCase));
            }
        }

        public ObservableCollection<BookDto> Books 
        {
            get => _books;
            set
            {
                _books = value;
                OnPropertyChanged();
            }
        }
        public MainWindowViewModel() : base(null)
        {
        }

        public MainWindowViewModel(IWindowController windowController) : base(windowController)
        {
            LoadCommands();
        }

        private void LoadCommands()
        {
        }

        /// <summary>
        /// Lädt die gefilterten Buchdaten
        /// </summary>
        public async Task LoadBooks()
        {
            await using(UnitOfWork uow = new UnitOfWork())
            {
                _books = new ObservableCollection<BookDto>(await uow.Books.GetAllBookDtosAsync());
            }
            _view = (CollectionView)CollectionViewSource.GetDefaultView(Books);
            _view.Filter = BookFilter;
        }

        public static async Task<BaseViewModel> Create(IWindowController controller)
        {
            var model = new MainWindowViewModel(controller);
            await model.LoadBooks();
            return model;
        }

        private ICommand _cmdNewBook;
        public ICommand CmdNewBook 
        { 
            get
            {
                if(_cmdNewBook == null)
                {
                    _cmdNewBook = new RelayCommand(
                        execute: async _ =>
                        {
                            SelectedBook = null;
                            Controller.ShowWindow(await BookEditCreateViewModel.Create(Controller, SelectedBook), true);
                            await LoadBooks();
                        },
                        canExecute: _ => SelectedBook == null);
                }

                return _cmdNewBook;
            }
            set
            {
                _cmdNewBook = value;
            }
        }

        private ICommand _cmdEditBook;
        public ICommand CmdEditBook
        {
            get
            {
                if (_cmdEditBook == null)
                {
                    _cmdEditBook = new RelayCommand(
                        execute: async _ =>
                        {
                            Controller.ShowWindow(await BookEditCreateViewModel.Create(Controller, SelectedBook), true);
                            SelectedBook = null;
                            await LoadBooks();
                        },
                        canExecute: _ => SelectedBook != null);
                }

                return _cmdEditBook;
            }
            set
            {
                _cmdEditBook = value;
            }
        }

        private ICommand _cmdDeleteBook;
        public ICommand CmdDeleteBook 
        { 
            get
            {
                if(_cmdDeleteBook == null)
                {
                    _cmdDeleteBook = new RelayCommand(
                        execute: async _ =>
                        {
                            await using (UnitOfWork uow = new UnitOfWork())
                            {
                                var tmpBook = await uow.Books.LookUpBookAsync(SelectedBook.Title);

                                uow.Books.RemoveBook(tmpBook[0]);
                                Books.Remove(SelectedBook);
                                await uow.SaveChangesAsync();
                            }
                        },
                        canExecute: _ => SelectedBook != null);
                }
                return _cmdDeleteBook;
            }
            set
            {
                _cmdDeleteBook = value;
            }
        }

        public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            throw new NotImplementedException();
        }
    }
}
