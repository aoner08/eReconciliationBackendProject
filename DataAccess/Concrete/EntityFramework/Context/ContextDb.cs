﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Entities.Concrete;
using Entities.Concrete;
using Microsoft.EntityFrameworkCore;


namespace DataAccess.Concrete.EntityFramework.Context
{
	public class ContextDb : DbContext
	{
		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			optionsBuilder.UseSqlServer(@"Server=aOner\db;Database=eReconciliationDb;Integrated Security=true");
			//Security=true ile bağlantının güvenli olup olmadığını belirliyoruz, makina local ise true yapıyoruz, sunucu üzerinden bağlantıyı kurmamız durumunda false yaparız
			base.OnConfiguring(optionsBuilder);
		}
		public DbSet<AccountReconciliationDetail> AccountReconciliationDetails { get; set; }
		public DbSet<AccountReconciliation> AccountReconciliations { get; set; }
		public DbSet<BaBsReconciliation> BaBsReconciliations { get; set; }
		public DbSet<BaBsReconciliationDetail> BaBsReconciliationDetails { get; set; }
		public DbSet<Company> Companies { get; set; }
		public DbSet<Currency> Currencies { get; set; }
		public DbSet<CurrencyAccount> CurrencyAccounts { get; set; }
		public DbSet<MailParameter> MailParameters { get; set; }
		public DbSet<OperationClaim> OperationClaims { get; set; }
		public DbSet<User> Users { get; set; }
		public DbSet<UserCompany> UserCompanies { get; set; }
		public DbSet<UserOperationClaim> UserOperationClaims { get; set; }
		public DbSet<MailTemplate> MailTemplate { get; set; }


	}
}
