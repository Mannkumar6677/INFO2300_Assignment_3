﻿using BOMLink.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BOMLink.ViewModels {
    public class JobViewModel {
        // Used for listing (Index)
        public List<Job> Jobs { get; set; } = new();

        public string SearchTerm { get; set; }
        public string SelectedCustomer { get; set; }
        public string SortBy { get; set; }
        public string SortOrder { get; set; }
        public string Action { get; set; }

        public List<Customer> Customers { get; set; } = new();
    }
}
