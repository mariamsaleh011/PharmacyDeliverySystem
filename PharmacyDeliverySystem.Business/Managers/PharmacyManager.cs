<<<<<<< HEAD
﻿using System;
using System.Collections.Generic;
=======
﻿using System.Collections.Generic;
>>>>>>> upstream/Kamal-Branch
using System.Linq;
using Microsoft.EntityFrameworkCore;
using PharmacyDeliverySystem.Business.Interfaces;
using PharmacyDeliverySystem.DataAccess;
using PharmacyDeliverySystem.Models;

namespace PharmacyDeliverySystem.Business.Managers
{
    public class PharmacyManager : IPharmacyManager
    {
        private readonly PharmacyDeliveryContext _context;

        public PharmacyManager(PharmacyDeliveryContext context)
        {
            _context = context;
        }

        public IEnumerable<Pharmacy> GetAllPharmacies()
        {
            return _context.Pharmacies.ToList();
        }

        public Pharmacy? GetById(int id)
        {
            return _context.Pharmacies.FirstOrDefault(p => p.PharmId == id);
        }

        public IEnumerable<Pharmacy> GetByName(string name)
        {
            return _context.Pharmacies
                .Where(p => p.Name.Contains(name))
                .ToList();
        }

        public void Create(Pharmacy pharmacy)
        {
            _context.Pharmacies.Add(pharmacy);
            _context.SaveChanges();
        }

        public void Update(Pharmacy pharmacy)
        {
            _context.Pharmacies.Update(pharmacy);
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var pharmacy = _context.Pharmacies.FirstOrDefault(p => p.PharmId == id);
            if (pharmacy == null) return;

            _context.Pharmacies.Remove(pharmacy);
            _context.SaveChanges();
        }
<<<<<<< HEAD

        // For Pharmacy Chat
        public List<Chat> GetChatsByPharmacyId(int pharmacyId)
=======
        // For Pharmacy Chat
            public List<Chat> GetChatsByPharmacyId(int pharmacyId)
>>>>>>> upstream/Kamal-Branch
        {
            return _context.Chats
                .Where(c => c.PharmacyId == pharmacyId)
                .Include(c => c.Customer)
                .Include(c => c.ChatMessages)
                .ToList();
        }
<<<<<<< HEAD

        public Chat? GetChatById(int chatId)
=======
        public Chat GetChatById(int chatId)
>>>>>>> upstream/Kamal-Branch
        {
            return _context.Chats
                .Include(c => c.Customer)
                .Include(c => c.ChatMessages)
                .FirstOrDefault(c => c.ChatId == chatId);
        }
<<<<<<< HEAD

=======
>>>>>>> upstream/Kamal-Branch
        public void SendMessage(int chatId, string message, string senderType)
        {
            var newMessage = new ChatMessage
            {
                ChatId = chatId,
                MessageText = message,
                SenderType = senderType,
                SentAt = DateTime.Now
            };

            _context.ChatMessages.Add(newMessage);
            _context.SaveChanges();
        }
<<<<<<< HEAD

        public Pharmacy? GetPharmacyByEmail(string email)
        {
            return _context.Pharmacies.FirstOrDefault(p => p.Email == email);
        }
=======
        public Pharmacy GetPharmacyByEmail(string email)
        {
            return _context.Pharmacies.FirstOrDefault(p => p.Email == email);
        }

>>>>>>> upstream/Kamal-Branch
    }
}

