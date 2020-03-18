using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using Evidos.EventSourcing.EventHandling.Abstractions;
using System.Threading;

namespace Evidos.EventSourcing.ConsoleApp
{
    public class ConsoleApplication
    {
        private readonly IUserReader _userReader;
        private readonly IUserWriter _userWriter;
        private readonly Lazy<HashSet<string>> _emailAddresses = new Lazy<HashSet<string>>(() => new HashSet<string>());
        private readonly ReaderWriterLockSlim _emailAddressesLock = new ReaderWriterLockSlim();

        public ConsoleApplication(
            IUserReader userReader,
            IUserWriter userWriter)
        {
            _userReader = userReader;
            _userWriter = userWriter;

            _userReader.QueryAsync(user => true).GetAwaiter().GetResult()
                .Select(user => user.EmailAddress)
                .ToList()
                .ForEach(emailAddress =>
                    _emailAddresses.Value.Add(emailAddress)
                );
        }

        public void Run()
        {
            ShowHelp();

            while (true)
            {
                var line = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(line))
                {
                    Console.WriteLine("Please type a command\n");
                    continue;
                }

                var args = line.Split(" ");

                switch (args[0])
                {
                    case "add":
                        AddUser(args);
                        break;
                    case "update":
                        UpdateUser(args);
                        break;
                    case "delete":
                        DeleteUser(args);
                        break;
                    case "verify":
                        VerifyUser(args);
                        break;
                    case "search":
                        SearchUsers(args);
                        break;
                    case "help":
                        ShowHelp();
                        break;
                    default:
                        Console.WriteLine("Unrecognized command\n");
                        ShowHelp();
                        break;
                }
            }

            void ShowHelp()
            {
                Console.WriteLine("Commands:");
                Console.WriteLine("\tadd [emailAddress]\t\t- Add a user by e-mail address");
                Console.WriteLine("\tupdate [id] [emailAddress]\t- Update an existing user's e-mail address");
                Console.WriteLine("\tdelete [id]\t\t\t- Delete a user");
                Console.WriteLine("\tverify [id]\t\t\t- Verify a user");
                Console.WriteLine("\tsearch [emailAddress]\t\t- Search users by (partial) e-mail address\n");
            }
        }

        private void AddUser(string[] args)
        {
            if (args.Length < 2 || string.IsNullOrWhiteSpace(args[1]))
            {
                Console.WriteLine($"Please provide an e-mail address as argument\n");
                return;
            }
            
            if (!CanClaimEmailAddress(args[1]))
                return;
            
            if (!IsValidEmailAddress(args[1]))
                return;

            _userWriter.CreateAsync(args[1].Trim());
            Console.WriteLine($"E-mail address submitted\n");
        }

        private void SearchUsers(string[] args)
        {
            if (args.Length < 2 || string.IsNullOrWhiteSpace(args[1]))
            {
                Console.WriteLine($"Please provide an e-mail address (partially) as argument\n");
                return;
            }

            var users = _userReader.QueryAsync((user) => user.EmailAddress.Contains(args[1].Trim()))
                .GetAwaiter().GetResult().ToList();

            if (users?.FirstOrDefault() == null)
                Console.WriteLine($"No users found\n");
            else
                users.ForEach(user =>
                {
                    Console.WriteLine($"{user.Id}: {user.EmailAddress} (status: {user.Status})");
                });

            Console.WriteLine("\n");
        }

        private void DeleteUser(string[] args)
        {
            if (args.Length < 2 || string.IsNullOrWhiteSpace(args[1]))
            {
                Console.WriteLine($"Please provide an id as argument\n");
                return;
            }
            
            if (!Guid.TryParse(args[1].Trim(), out Guid id))
            {
                Console.WriteLine($"Please provide a valid id as argument\n");
                return;
            }

            _userWriter.DeleteAsync(id);
            Console.WriteLine($"User delete operation submitted\n");
        }

        private void VerifyUser(string[] args)
        {
            if (args.Length < 2 || string.IsNullOrWhiteSpace(args[1]))
            {
                Console.WriteLine($"Please provide an id as argument\n");
                return;
            }
            
            if (!Guid.TryParse(args[1].Trim(), out Guid id))
            {
                Console.WriteLine($"Please provide a valid id as argument\n");
                return;
            }

            _userWriter.VerifyAsync(id);
            Console.WriteLine($"User verification submitted\n");
        }

        private void UpdateUser(string[] args)
        {
            if (args.Length < 3 || args.Any(string.IsNullOrWhiteSpace))
            {
                Console.WriteLine($"Please provide an id and an e-mail address argument\n");
                return;
            }
            
            if (!Guid.TryParse(args[1].Trim(), out Guid id))
            {
                Console.WriteLine($"Please provide a valid id as argument\n");
                return;
            }
            
            if (!IsValidEmailAddress(args[2]))
                return;

            if (!CanClaimEmailAddress(args[2]))
                return;

            _userWriter.UpdateAsync(id, args[2]);
            Console.WriteLine($"E-mail address update submitted\n");
        }

        private bool IsValidEmailAddress(string emailAddress)
        {
            try
            {
                var _ = new MailAddress(emailAddress);

                return true;
            }
            catch (FormatException)
            {
                Console.WriteLine("Invalid e-mail address\n");
                return false;
            }
        }

        private bool CanClaimEmailAddress(string emailAddress)
        {
            if (_emailAddressesLock.TryEnterWriteLock(TimeSpan.FromSeconds(3)))
            {
                try
                {
                    var canClaim = _emailAddresses.Value.Add(emailAddress);

                    if (!canClaim)
                        Console.WriteLine("E-mail address already exists\n");

                    return canClaim;
                }
                finally
                {
                    _emailAddressesLock.ExitWriteLock();
                }
            }
            Console.WriteLine("Error claiming e-mail address\n");
            return false;
        }
    }
}
