using System;
using System.Collections.Generic;
class Book
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Author { get; set; }
    public string Genre { get; set; }
    public bool IsAvailable { get; set; } = true;
    public Book(int id, string title, string author, string genre)
    {
        Id = id;
        Title = title;
        Author = author;
        Genre = genre;
    }
}
class User
{
    public int UserID { get; set; }
    public string UserName { get; set; }
    public BarrowedBooks barrowedBooks { get; set; }
    public User(int userId, string userName)
    {
        UserID = userId;
        UserName = userName;
    }
}
class BarrowedBooks
{
    public User User { get; set; }
    public Book Book { get; set; }
    public BarrowedBooks(User user, Book book)
    {
        User = user;
        Book = book;
    }
}
class Librarymanagement
{
    private List<Book> books = new List<Book>();
    private List<User> users = new List<User>();
    private Dictionary<User, List<Book>> barrowedbyUser = new Dictionary<User, List<Book>>();
    private List<BarrowedBooks> borrowRecords = new List<BarrowedBooks>();
    public void AddBook(Book book)
    {
        books.Add(book);
        Console.WriteLine($"{book.Title} added successfully");
    }
    public void AddUser(User user)
    {
        users.Add(user);
        Console.WriteLine($"{user.UserName} added successfully");
    }
    public void RemoveBook(int id)
    {
        Book book = books.Find(b => b.Id == id);
        if (book != null)
        {
            books.Remove(book);
            Console.WriteLine($"{book.Title} is Removed");
        }
        else
        {
            Console.WriteLine("Invalid book Id");
        }
    }
    public void Search(string title)
    {
        List<Book> booktoSearch = books.FindAll(b => b.Title.ToLower().Contains(title.ToLower()));
        if (booktoSearch.Count == 0)
        {
            Console.WriteLine("Sorry the book you are searching is not available here");
        }
        else
        {
            Console.WriteLine("Your search Matches with:");
            foreach (var b in booktoSearch)
            {
                Console.WriteLine($"Title:{b.Title} by {b.Author}");
            }
        }
    }
    public void DisplayBooks()
    {
        if (books.Count > 0)
        {
            Console.WriteLine("Books Available:");
            foreach (Book book in books)
            {
                if (book.IsAvailable)
                {
                    Console.WriteLine($"Title:{book.Title} - Author:{book.Author} ");
                }
            }
        }
    }
    public void BookToBarrow(int userId, int bookId)
    {
        Book bookTobarrow = books.Find(b => b.Id == bookId);
        User UserToBorrow = users.Find(u => u.UserID == userId);
        if (UserToBorrow == null)
        {
            Console.WriteLine("User Not Found");
            return;
        }
        else if (bookTobarrow == null)
        {
            Console.WriteLine($"Sorry, {bookTobarrow.Title} is Not available in our libary");
            return;
        }
        if (!bookTobarrow.IsAvailable)
        {
            Console.WriteLine($"{bookTobarrow.Title} is Not available");
            return;
        }
        if (!barrowedbyUser.ContainsKey(UserToBorrow))
        {
            barrowedbyUser[UserToBorrow] = new List<Book>();
        }
        bookTobarrow.IsAvailable = false;
        barrowedbyUser[UserToBorrow].Add(bookTobarrow);
        //borrowRecords.Add(new BarrowedBooks {User=UserToBorrow,Book= bookTobarrow });
        Console.WriteLine($"{bookTobarrow.Title} borrowed by {UserToBorrow.UserName}");
    }
    public void BookToReturn(int userId, int bookId)
    {
        var UserToReturn = users.Find(u => u.UserID == userId);
         Book bookToReturn = books.Find(b => b.Id == bookId);
        if (bookToReturn == null)
        {
            Console.WriteLine($"Sorry, {bookToReturn.Title} is Not belongs to our libary");
            return;
        }
        else if (UserToReturn == null)
        {
            Console.WriteLine("User Not Found");
            return;
        }
        if (!barrowedbyUser.ContainsKey(UserToReturn))
        {
            Console.WriteLine($"{UserToReturn.UserName} not found in the borrowed user list");
            return;
        }
        bookToReturn.IsAvailable = true;
        barrowedbyUser[UserToReturn].Remove(bookToReturn);
        Console.WriteLine($"{bookToReturn.Title} Returned by {UserToReturn.UserName}");
    }
}
class Program
{
    static void Main(string[] args)
    {
        Librarymanagement librarymanagement = new Librarymanagement();
        while (true)
        {
            Console.WriteLine("1. To add Book to list of books");
            Console.WriteLine("2. To View available books ");
            Console.WriteLine("3. To borrow book");
            Console.WriteLine("4. To return the Book ");
            Console.WriteLine("5. To Search for book details ");
            Console.WriteLine("6. To remove a book");
            Console.WriteLine("7. To add user");
            Console.WriteLine("8. To exit ");
            int choice = int.Parse(Console.ReadLine());
            switch (choice)
            {
                case 1:
                    Console.WriteLine("Enter the Book Id, Title, Author,genre");
                    int bookId = int.Parse(Console.ReadLine());
                    string title = Console.ReadLine();
                    string author = Console.ReadLine();
                    string genre = Console.ReadLine();
                    librarymanagement.AddBook(new Book(bookId, title, author, genre));
                    break;
                case 2:
                    librarymanagement.DisplayBooks();
                    break;
                case 3:
                    Console.WriteLine("Enter user id:");
                    int userId = int.Parse(Console.ReadLine());
                    Console.WriteLine("Enter Book Id:");
                    int id = int.Parse(Console.ReadLine());
                    try
                    {
                        librarymanagement.BookToBarrow(userId, id);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Book already Issued"+ex.Message);
                    }
                    break;
                case 4:
                    Console.WriteLine("Enter user id:");
                    int usId = int.Parse(Console.ReadLine());
                    Console.WriteLine("Enter Book Id:");
                    int boId = int.Parse(Console.ReadLine());
                    librarymanagement.BookToReturn(usId, boId);
                    break;
                case 5:
                    Console.WriteLine("Enter Book title:");
                    string ti = Console.ReadLine();
                    librarymanagement.Search(ti);
                    break;
                case 6:
                    Console.WriteLine("Enter Book Id:");
                    int booId = int.Parse(Console.ReadLine());
                    try
                    {
                        librarymanagement.RemoveBook(booId);
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("Book id not found");
                    }
                    break;
                case 7:
                    Console.WriteLine("Enter the UserID, UserName");
                    int uId = int.Parse(Console.ReadLine());
                    string userName = Console.ReadLine();
                    librarymanagement.AddUser(new User(uId, userName));
                    break;
                case 8:
                    Console.WriteLine("Existing the Library...");
                    return;
                default:
                    Console.WriteLine("Invalid Choice");
                    break;
            }
        }
    }
}