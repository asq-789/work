﻿using System;
using System.Collections.Generic;

namespace _2302b1TempEmbedding.Models;

public partial class User
{
    public int Id { get; set; }

    public string Username { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public int Status { get; set; }

    public int Roleid { get; set; }

    public virtual ICollection<Cart> Carts { get; set; } = new List<Cart>();
}
