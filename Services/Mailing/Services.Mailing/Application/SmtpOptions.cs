﻿namespace Services.Mailing.Application;

public class SmtpOptions
{
    public string Server { get; set; }
    public int Port { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
}
