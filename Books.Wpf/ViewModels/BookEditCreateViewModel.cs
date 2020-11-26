using Books.Core.DataTransferObjects;
using Books.Core.Entities;
using Books.Core.Validations;
using Books.Persistence;
using Books.Wpf.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Books.Wpf.ViewModels
{
    public class BookEditCreateViewModel : BaseViewModel
    {
        private string _selectedPublisher;
        private string _isbn;
        private string _title;
        private Dictionary<string, AuthorDto> _removedAuthors;
        private ObservableCollection<string> _selectedAuthors;
        private string _authorInListView;
        private AuthorDto _selectedAuthor;
        private ObservableCollection<AuthorDto> _authors;
        public string WindowTitle { get; set; }
        public string[] Publishers { get; set; }

        public string SelectedPublisher 
        {
            get => _selectedPublisher;
            set
            {
                _selectedPublisher = value;
                OnPropertyChanged();
            }
        }

        [IsbnValidation]
        public string Isbn 
        {
            get => _isbn;
            set
            {
                _isbn = value;
                OnPropertyChanged();
                ValidateViewModelProperties();
            }
        }

        [Required(ErrorMessage ="Titel muss angegeben werden!", AllowEmptyStrings = false)]
        public string Title 
        {
            get => _title;
            set
            {
                _title = value;
                OnPropertyChanged();
                ValidateViewModelProperties();
            }
        }
        public ObservableCollection<AuthorDto> Authors 
        {
            get => _authors;
            set
            {
                _authors = value;
                OnPropertyChanged();
            }
        }

        public AuthorDto SelectedAuthor 
        {
            get => _selectedAuthor;
            set
            {
                _selectedAuthor = value;
                OnPropertyChanged();
            }
        }
        public string AuthorInListView
        {
            get => _authorInListView;
            set
            {
                _authorInListView = value;
                OnPropertyChanged();
            }
        }
        public ObservableCollection<string> SelectedAuthors 
        {
            get => _selectedAuthors;
            set
            {
                _selectedAuthors = value;
                OnPropertyChanged();
            }
        }

        private ICommand _cmdAddAuthor;
        public ICommand CmdAddAuthor 
        { 
            get
            {
                if(_cmdAddAuthor == null)
                {
                    _cmdAddAuthor = new RelayCommand(
                        execute: _ =>
                        {
                            SelectedAuthors.Add($"{SelectedAuthor.Author}");
                            var author = Authors.Where(a => a.Author == SelectedAuthor.Author).Select(a => a).FirstOrDefault();
                            _removedAuthors.Add(author.Author, author);
                            Authors.Remove(author);
                        },
                        canExecute: _ => SelectedAuthor != null);
                }
                return _cmdAddAuthor;
            }
            set
            {
                _cmdAddAuthor = value;
            }
        }

        private ICommand _cmdRemoveAuthor;
        public ICommand CmdRemoveAuthor
        {
            get
            {
                if (_cmdRemoveAuthor == null)
                {
                    _cmdRemoveAuthor = new RelayCommand(
                        execute: _ =>
                        {
                            var author = _removedAuthors[AuthorInListView];
                            Authors.Add(author);
                            SelectedAuthors.Remove(AuthorInListView);
                            var tmp = Authors.OrderBy(a => a.Author);
                            Authors = new ObservableCollection<AuthorDto>(tmp);
                        },
                        canExecute: _ => AuthorInListView != null);
                }
                return _cmdRemoveAuthor;
            }
            set
            {
                _cmdRemoveAuthor = value;
            }
        }

        private ICommand _cmdSaveBook;
        public ICommand CmdSaveBook 
        { 
            get
            {
                if(_cmdSaveBook == null)
                {
                    _cmdSaveBook = new RelayCommand(
                        execute: async _ =>
                        {
                            Book newBook = new Book()
                            {
                                BookAuthors = new List<BookAuthor>(),
                                Title = Title,
                                Isbn = Isbn,
                                Publishers = SelectedPublisher
                            };

                            foreach (var author in SelectedAuthors)
                            {
                                newBook.BookAuthors.Add(new BookAuthor()
                                {
                                    Author = new Author()
                                    {
                                        Name = author,
                                    },
                                    Book = newBook
                                });
                            }

                            try
                            {
                                await using (UnitOfWork uow = new UnitOfWork())
                                {
                                    await uow.Books.AddAsync(newBook);
                                    await uow.SaveChangesAsync();
                                }
                            }
                            catch(ValidationException ex)
                            {
                                DbError = ex.Message;
                            }
                        },
                        canExecute: _ => IsValid);
                }

                return _cmdSaveBook;
            }
            set
            {
                _cmdSaveBook = value;
            }
        }
        public BookEditCreateViewModel() : base(null)
        {
        }

        public BookEditCreateViewModel(IWindowController windowController, BookDto book) : base(windowController)
        {
            LoadCommands();
            ValidateViewModelProperties();
        }

        public static async Task<BaseViewModel> Create(IWindowController controller, BookDto book)
        {
            var model = new BookEditCreateViewModel(controller, book);
            await model.LoadData(book);
            return model;
        }

        private async Task LoadData(BookDto book)
        {
            await using (UnitOfWork uow = new UnitOfWork())
            {
                Authors = new ObservableCollection<AuthorDto>(await uow.Authors.GetAuthorDtosAsync());
                var tmp = Authors.OrderBy(a => a.Author);
                Authors = new ObservableCollection<AuthorDto>(tmp);
                Publishers = await uow.Books.GetAllPublishersAsync();
            }
            SelectedAuthors = new ObservableCollection<string>();
            _removedAuthors = new Dictionary<string, AuthorDto>();
            if (book == null)
            {
                WindowTitle = "Buch anlegen";
            }
            else
            {
                WindowTitle = "Buch bearbeiten";
                Title = book.Title;
                Isbn = book.Isbn;
                SelectedPublisher = book.Publishers;
                var authors = book.Authors.Split('/');

                foreach (var author in authors)
                {
                    SelectedAuthors.Add(author);
                    var tmp = Authors.Where(a => a.Author == author).Select(a => a).FirstOrDefault();
                    _removedAuthors.Add(author, tmp);
                    Authors.Remove(_removedAuthors[author]);
                }
            }
        }

        private void LoadCommands()
        {
        }

        public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            return Enumerable.Empty<ValidationResult>();
        }

    }
}
