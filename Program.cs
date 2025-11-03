using System;
using System.Collections.Generic;
using System.IO;
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
    public Book(int id, string title, string author, string genre, bool isAvailable)
    {
        Id = id;
        Title = title;
        Author = author;
        Genre = genre;
        IsAvailable = isAvailable;
    }
}
class User
{
    public int UserID { get; set; }
    public string UserName { get; set; }
    public List<Book> BarrowedBooks { get; set; } = new List<Book>();
    public User(int userId, string userName)
    {
        UserID = userId;
        UserName = userName;
    }
    public override string ToString()
    {
        return $"{UserID} | {UserName} | Borrowed: {BarrowedBooks.Count}";
    }
}


class Librarymanagement
{
    private List<Book> books = new List<Book>();
    private List<User> users = new List<User>();
    private Dictionary<User, List<Book>> barrowedbyUser = new Dictionary<User, List<Book>>();
    
    private string filePath = "borrowed_books.txt";
    private readonly object fileLock = new object();
    public void AddBook(Book book)
    {
        books.Add(book);
        Console.WriteLine($"{book.Title} added successfully");
        SaveBooksToFile();
    }
    public void AddUser(User user)
    {
        users.Add(user);
        Console.WriteLine($"{user.UserName} added successfully");
        SaveUsersToFile();
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
        lock (fileLock)
        {
            if (!barrowedbyUser.ContainsKey(UserToBorrow))
            {
                barrowedbyUser[UserToBorrow] = new List<Book>();
            }
            bookTobarrow.IsAvailable = false;
            barrowedbyUser[UserToBorrow].Add(bookTobarrow);
            SaveDataToFile();
            UserToBorrow.BarrowedBooks.Add(bookTobarrow);
            Console.WriteLine($"{bookTobarrow.Title} borrowed by {UserToBorrow.UserName}");
        }
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
        UserToReturn.BarrowedBooks.Remove(bookToReturn);
        SaveDataToFile();
        Console.WriteLine($"{bookToReturn.Title} Returned by {UserToReturn.UserName}");
    }
    public void SaveBooksToFile()
    {
        using (StreamWriter writer = new StreamWriter("books.txt"))
        {
            foreach (var book in books)
            {
                writer.WriteLine($"{book.Id},{book.Title},{book.Author},{book.Genre},{book.IsAvailable}");
            }
        }
    }
    public void SaveUsersToFile()
    {
        using (StreamWriter writer = new StreamWriter("users.txt"))
        {
            foreach (var user in users)
            {
                writer.WriteLine($"{user.UserID},{user.UserName}");
            }
        }
    }
    public void LoadBookData()
    {
        if (!File.Exists("books.txt"))
        {
            Console.WriteLine("There is not book file");
            return;
        }
        string[] lines = File.ReadAllLines("books.txt");
        foreach (var line in lines)
        {
            string[] parts = line.Split(",");
            int bookId = int.Parse(parts[0]);
            string title = parts[1];
            string author = parts[2];
            string genre = parts[3];
            bool IsAvailable = bool.Parse(parts[4]);
            books.Add(new Book(bookId, title, author, genre, IsAvailable));
        }
    }
    public void LoadUserData()
    {
        if (!File.Exists("users.txt"))
        {
            Console.WriteLine("There is not user file");
            return;
        }
        string[] lines = File.ReadAllLines("users.txt");
        foreach (var line in lines)
        {
            string[] parts = line.Split(",");
            int userId = int.Parse(parts[0]);
            string userName = parts[1];
            users.Add(new User(userId, userName));
        }
    }
    public void SaveDataToFile()
    {
        lock (fileLock)
        {
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                foreach (var entry in barrowedbyUser)
                {
                    foreach (var book in entry.Value)
                    {
                        writer.WriteLine($"{entry.Key.UserID},{entry.Key.UserName},{book.Id},{book.Title},{book.Author},{book.Genre}");
                    }
                }
            }
        }
    }
    public void LoadDataFromFile()
    {
        if (!File.Exists(filePath))
        {
            Console.WriteLine("File does not exist");
            return;
        }
        string[] lines = File.ReadAllLines(filePath);
        foreach (var line in lines)
        {
            string[] parts = line.Split(",");
            int userId = int.Parse(parts[0]);
            string userName = parts[1];
            int bookId = int.Parse(parts[2]);
            string title = parts[3];
            string author = parts[4];
            string genre = parts[5];
            User user = users.Find(u => u.UserID == userId);
            if (user == null)
            {
                user = new User(userId, userName);
                users.Add(user);
            }
            Book book = books.Find(b => b.Id == bookId);
            if (book == null)
            {
                book = new Book(bookId, title, author, genre);
                books.Add(book);
            }
            if (!barrowedbyUser.ContainsKey(user))
            {
                barrowedbyUser[user] = new List<Book>();
            }
            barrowedbyUser[user].Add(book);
        }

    }
    public void SimulateBorrowingBooks(List<(int userid, int bookId)> borrowRequests)
    {
        List<Task> tasks = new List<Task>();
        foreach (var requests in borrowRequests)
        {
            int userId = requests.userid;
            int bookId = requests.bookId;
            Task task = Task.Run(() =>
            {
                try
                {
                    BookToBarrow(userId, bookId);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error borrowing book {bookId} by user {userId}: {ex.Message}");
                }
            });
            tasks.Add(task);
        }
        Task.WaitAll(tasks.ToArray());
    }
}
class Program
{
    static void Main(string[] args)
    {
        Librarymanagement librarymanagement = new Librarymanagement();
        librarymanagement.LoadUserData();
        librarymanagement.LoadBookData();
        librarymanagement.LoadDataFromFile();
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
            Console.WriteLine("9. Simulate multiple borrow requests dynamically");
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
                        Console.WriteLine("Book already Issued" + ex.Message);
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
                case 9:
                    Console.WriteLine("How many borrow requests do you want to simulate?");
                    int count = int.Parse(Console.ReadLine());
                    List<(int userId, int bookId)> requsts = new List<(int userId, int bookId)>();
                    for (int i = 0; i < count; i++)
                    {
                        Console.WriteLine($"Enter User ID for request {i + 1}:");
                        int uid = int.Parse(Console.ReadLine());
                        Console.WriteLine($"Enter Book ID for request {i + 1}:");
                        int bid = int.Parse(Console.ReadLine());
                        requsts.Add((uid, bid));
                    }
                    librarymanagement.SimulateBorrowingBooks(requsts);
                    break;
                default:
                    Console.WriteLine("Invalid Choice");
                    break;
            }
        }
    }
}