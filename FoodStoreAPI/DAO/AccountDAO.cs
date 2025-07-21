using AutoMapper;
using FoodStoreAPI.DTO;
using FoodStoreAPI.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace FoodStoreAPI.DAO
{
    public class AccountDAO
    {
        private static readonly IMapper _mapper;

        static AccountDAO()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Account, AccountDTO>();
                cfg.CreateMap<Account, AccountLDTO>();
                cfg.CreateMap<Role, RoleDTO>();
            });
            _mapper = config.CreateMapper();
        }

        public static AccountLDTO Login(string email, string password)
        {
            
            string passHash = HashPassword(password);
            Account account = FoodStoreContext.Ins.Accounts.Include(x => x.Role).
                FirstOrDefault(x => x.Email == email && x.Password == passHash);
            if (account != null)
            {
                return _mapper.Map<AccountLDTO>(account);
            }
            return null;
        }

        public static string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                foreach (byte b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }
                return builder.ToString();
            }
        }

        public static void Register(AccountDTO accountdto)
        {
            try
            {
                if (string.IsNullOrEmpty(accountdto.Email) || string.IsNullOrEmpty(accountdto.Password))
                {
                    throw new ArgumentException("Email và Password không được để trống.");
                }
                Account account = new Account
                {
                    Email = accountdto.Email,
                    Phone = accountdto.Phone,
                    Name = accountdto.Name,
                    Username = accountdto.Username,
                    Password = HashPassword(accountdto.Password),
                    CreateAt = DateTime.Now,
                    RoleId = 3,
                    Address = accountdto.Address
                };
                var context = FoodStoreContext.Ins;
                context.Accounts.Add(account);
                context.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                throw new Exception($"Lỗi Database: {ex.InnerException?.Message}");
            }
            catch (Exception e)
            {
                throw new Exception($"Lỗi hệ thống: {e.Message}");
            }
        }

        public static Account findAccountByUsernameAndEmail(string username, string email)
        {
            Account account = new Account();
            try
            {
                account = FoodStoreContext.Ins.Accounts.FirstOrDefault(x => x.Username == username && x.Email == email);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return account;
        }
        public static List<AccountLDTO> getAccount()
        {
            FoodStoreContext context = new FoodStoreContext();
            var listAccount = new List<AccountLDTO>();
            try
            {
                listAccount = context.Accounts
                    .Include(x => x.Role)
                    .Select(x => new AccountLDTO
                    {
                        AccId = x.AccId,
                        Username = x.Username,
                        CreateAt = x.CreateAt,
                        RoleId = x.RoleId,
                        Email = x.Email,
                        Phone = x.Phone,
                        RoleName = x.Role.RoleName
                    })
                    .ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return listAccount;
        }
        public static List<AccountLDTO> getAccountSearch(string searchAccount, string sort)
        {
            FoodStoreContext context = new FoodStoreContext();
            var listAccount = new List<AccountLDTO>();
            try
            {
                var query = context.Accounts.Include(x => x.Role).AsQueryable();

                // Search
                if (!string.IsNullOrWhiteSpace(searchAccount))
                {
                    searchAccount = searchAccount.ToLower();
                    query = query.Where(x =>
                        x.Username.ToLower().Contains(searchAccount.ToLower()) ||
                        x.Email.ToLower().Contains(searchAccount) ||
                        x.Phone.Contains(searchAccount)
                    );
                }

                // Sort
                switch (sort)
                {
                    case "fullnameAsc":
                        query = query.OrderBy(x => x.Username);
                        break;
                    case "fullnameDesc":
                        query = query.OrderByDescending(x => x.Username);
                        break;
                    case "emailAsc":
                        query = query.OrderBy(x => x.Email);
                        break;
                    case "emailDesc":
                        query = query.OrderByDescending(x => x.Email);
                        break;
                    default:
                        query = query.OrderBy(x => x.AccId); // Default sort
                        break;
                }

                listAccount = query.Select(x => new AccountLDTO
                {
                    AccId = x.AccId,
                    Username = x.Username,
                    CreateAt = x.CreateAt,
                    RoleId = x.RoleId,
                    Email = x.Email,
                    Phone = x.Phone,
                    RoleName = x.Role.RoleName
                }).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return listAccount;
        }

        public static void UpdateRoleAccount(int accId, int roleId)
        {
            FoodStoreContext context = new FoodStoreContext();
            try
            {
                Account acc = context.Accounts.SingleOrDefault(x => x.AccId == accId);
                acc.UpdateAt = DateTime.Now;
                acc.RoleId = roleId;
                context.Accounts.Update(acc);
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public static AccountDTO findAccountByEmail(string email)
        {
            Account account = new Account();
            try
            {
                account = FoodStoreContext.Ins.Accounts.FirstOrDefault(x => x.Email == email);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return _mapper.Map<AccountDTO>(account);
        }

        public static void changePassword(string email, string password)
        {
            Account account = FoodStoreContext.Ins.Accounts.FirstOrDefault(x => x.Email.Equals(email));
            if (account != null)
            {
                try
                {
                    String passHash = HashPassword(password);
                    account.Password = passHash;
                    account.UpdateAt = DateTime.Now;
                    FoodStoreContext.Ins.Accounts.Update(account);
                    FoodStoreContext.Ins.SaveChanges();
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
        }

        public static void updateInformationAccountById(int id, AccountDTO accountdto)
        {
            Account account = FoodStoreContext.Ins.Accounts.FirstOrDefault(x => x.AccId == id);
            if (account != null)
            {
                try
                {
                    account.Phone = accountdto.Phone;
                    account.Name = accountdto.Name;
                    account.UpdateAt = DateTime.Now;
                    account.Address = accountdto.Address;
                    FoodStoreContext.Ins.Accounts.Update(account);
                    FoodStoreContext.Ins.SaveChanges();
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
        }

        public static Account findAccountById(int id)
        {
            Account account = new Account();
            try
            {
                account = FoodStoreContext.Ins.Accounts.Include(x => x.Role).FirstOrDefault(x => x.AccId == id);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return account;
        }

        public static void AddNewUser(AccountDTO accountdto)
        {
            var isEmailExisted = FoodStoreContext.Ins.Accounts.Any(x => x.Email.Equals(accountdto.Email));
            if (isEmailExisted)
            {
                throw new ArgumentException("Email is existed!");
            }
            else
            {
                if (string.IsNullOrEmpty(accountdto.Name))
                {
                    throw new ArgumentException("Your name must be valid!");
                }
                else
                {
                    if (!Regex.IsMatch(accountdto.Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                    {
                        throw new ArgumentException("Invalid email or email early exist!");
                    }
                    else
                    {
                        if (accountdto.Phone.Length != 10 && !Regex.IsMatch(accountdto.Phone, @"^(03|05|07|08|09)\d{8}$"))
                        {
                            throw new ArgumentException("Phone number must be 10 digits, and have a matching prefix!");
                        }
                        else
                        {
                            bool isValidPassword = accountdto.Password.Length > 8 && Regex.IsMatch(accountdto.Password, @"^[A-Z]") && Regex.IsMatch(accountdto.Password, @"\d") && Regex.IsMatch(accountdto.Password, @"[!@#$%^&*(),.?:{}|<>]");
                            if (string.IsNullOrEmpty(accountdto.Password) || !isValidPassword)
                            {
                                throw new ArgumentException("Password must be at least 8 digits, must have an uppercase letter at the beginning of the string, must have at least one number, and must have at least one special character!");
                            }
                            else
                            {
                                Account account = new Account
                                {
                                    Email = accountdto.Email,
                                    Phone = accountdto.Phone,
                                    Name = accountdto.Name,
                                    Username = accountdto.Username,
                                    Password = HashPassword(accountdto.Password),
                                    CreateAt = DateTime.Now,
                                    RoleId = accountdto.RoleId,
                                    Address = accountdto.Address
                                };
                                var context = FoodStoreContext.Ins;
                                context.Accounts.Add(account);
                                context.SaveChanges();
                            }
                        }
                    }
                }                  
            }            
        }

    }
}
