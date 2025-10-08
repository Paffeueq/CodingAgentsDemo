// Simple console login demo with validation and a tiny in-memory authenticator.
// - ValidateCredentials: checks username and password shape and returns a helpful error message.
// - Authenticate: example in-memory user store (do NOT use in production).
// The program supports two modes:
// 1) CLI test mode: pass username and password as command-line arguments (for automated smoke tests)
// 2) Interactive mode: prompts the user for username and password

// Moduł: Prosty demo logowania
//
// Cel:
// - Pokazać prosty proces walidacji danych logowania oraz przykładowe uwierzytelnianie
//   w konsolowej aplikacji demonstracyjnej.
//
// Co zawiera kod:
// - ValidateCredentials(username, password, out error): waliduje nazwę użytkownika i hasło
//   według reguł opisanych poniżej i zwraca czytelny komunikat o błędzie.
// - Authenticate(username, password): prosty, lokalny sklep użytkowników (in-memory)
//   użyty tylko do demonstracji. Nie stosować w produkcji.
// - Tryb CLI: jeśli program uruchomiony z dwoma argumentami, traktuje je jako
//   username i password (przydatne do automatycznych testów).
// - Tryb interaktywny: wczytuje username i hasło od użytkownika; hasło jest wczytywane bez echa.
//
// Reguły walidacji (domyślne):
// - username: wymagany, min. 3 znaki, dozwolone znaki: litery, cyfry, '.', '_' i '-'.
// - password: wymagany, min. 8 znaków, musi zawierać co najmniej jedną dużą literę,
//   jedną małą literę, jedną cyfrę oraz jeden znak specjalny.
//
// Uwaga i bezpieczeństwo:
// - Ten przykład przechowuje hasła jawnie w pamięci wyłącznie do celów demonstracyjnych.
//   W środowisku produkcyjnym użyj silnego hashowania haseł (Argon2, bcrypt) i bezpiecznego
//   repozytorium użytkowników (baza danych).
// - Rozważyć dodatkowe zabezpieczenia: limit prób logowania, blokada konta, rate-limiting,
//   logowanie zdarzeń i szyfrowanie kanału komunikacji.
//
// Szybkie przykłady uruchomienia:
// - Tryb testowy (argumenty):
//   dotnet run --project <ścieżka-do-projektu> alice P@ssw0rd1
// - Tryb interaktywny:
//   dotnet run --project <ścieżka-do-projektu>

using System.Text.RegularExpressions;

Console.WriteLine("Login demo\n");

string username;
string password;

if (args.Length >= 2)
{
	// Test mode (useful for automated runs): first two args are username and password
	username = args[0];
	password = args[1];
	Console.WriteLine($"(Using args) Username: {username}");
}
else
{
	Console.Write("Username: ");
	username = Console.ReadLine() ?? string.Empty;
	Console.Write("Password: ");
	password = ReadPassword();
	Console.WriteLine();
}

if (!ValidateCredentials(username, password, out var error))
{
	Console.WriteLine($"Validation failed: {error}");
	return;
}

if (Authenticate(username, password))
{
	Console.WriteLine("Login successful");
}
else
{
	Console.WriteLine("Invalid credentials");
}

// -- Functions ------------------------------------------------------------

// Contract:
// - Inputs: username (string), password (string)
// - Output: bool (valid/invalid) and out string error (null when valid)
// - Error modes: returns false with an error message for empty/invalid inputs
static bool ValidateCredentials(string username, string password, out string? error)
{
	// Basic checks
	if (string.IsNullOrWhiteSpace(username))
	{
		error = "Username is required.";
		return false;
	}

	if (username.Length < 3)
	{
		error = "Username must be at least 3 characters long.";
		return false;
	}

	// Only allow common username characters (adjust as needed)
	if (!Regex.IsMatch(username, "^[a-zA-Z0-9._-]+$"))
	{
		error = "Username contains invalid characters. Use letters, digits, '.', '_' or '-'.";
		return false;
	}

	if (string.IsNullOrEmpty(password))
	{
		error = "Password is required.";
		return false;
	}

	if (password.Length < 8)
	{
		error = "Password must be at least 8 characters long.";
		return false;
	}

	if (!Regex.IsMatch(password, "[A-Z]"))
	{
		error = "Password must contain at least one uppercase letter.";
		return false;
	}

	if (!Regex.IsMatch(password, "[a-z]"))
	{
		error = "Password must contain at least one lowercase letter.";
		return false;
	}

	if (!Regex.IsMatch(password, "[0-9]"))
	{
		error = "Password must contain at least one digit.";
		return false;
	}

	if (!Regex.IsMatch(password, "[\\W_]"))
	{
		error = "Password should contain at least one special character.";
		return false;
	}

	error = null;
	return true;
}

// Very small example authenticator. Replace with a proper user store and secure hashing in real apps.
static bool Authenticate(string username, string password)
{
	var users = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
	{
		{ "alice", "P@ssw0rd1" },
		{ "bob", "Secret#123" }
	};

	return users.TryGetValue(username, out var stored) && stored == password;
}

// Utility to read password without echoing to the console
static string ReadPassword()
{
	var pass = string.Empty;
	ConsoleKeyInfo key;
	while ((key = Console.ReadKey(true)).Key != ConsoleKey.Enter)
	{
		if (key.Key == ConsoleKey.Backspace)
		{
			if (pass.Length > 0)
			{
				pass = pass[..^1];
				Console.Write("\b \b");
			}
		}
		else if (!char.IsControl(key.KeyChar))
		{
			pass += key.KeyChar;
			Console.Write("*");
		}
	}

	return pass;
}

