using DB.EFModel;
using DB.Entity;
namespace DB.Migrations.Helpers
{
    public class EmailHelper
    {
        public static string GetWelcomeEmailBody(string residentFullName, string residentEmail, string randomPassword, string residentPageUrl, string community)
        {


            //string htmlTemplate = @"
            //                            <!DOCTYPE html>
            //                            <html lang='en'>
            //                            <head>
            //                              <meta charset='UTF-8' />
            //                              <meta name='viewport' content='width=device-width, initial-scale=1.0' />
            //                              <title>Email Notification</title>
            //                              <style>
            //                                body {{
            //                                  font-family: Roboto, sans-serif;
            //                                  background-color: #f5f7fa;
            //                                  margin: 0;
            //                                  padding: 0;
            //                                }}
            //                                .email-container {{
            //                                  max-width: 600px;
            //                                  margin: 0 auto;
            //                                  background-color: #ffffff;
            //                                  border-radius: 6px;
            //                                  overflow: hidden;
            //                                }}
            //                                .header {{
            //                                  background-color: #1ea4aa;
            //                                  color: #ffffff;
            //                                  padding: 20px;
            //                                  text-align: center;
            //                                  font-size: 20px;
            //                                  font-weight: bold;
            //                                }}
            //                                .content {{
            //                                  padding: 30px;
            //                                }}
            //                                .content h2 {{
            //                                  margin-top: 0;
            //                                  color: #2b2b2b;
            //                                }}
            //                                .content p, .notification-box p, .footer p {{
            //                                  line-height: 1.5;
            //                                }}
            //                                .notification-box {{
            //                                  background-color: #f1f3f6;
            //                                  border-left: 4px solid #1ea4aa;
            //                                  padding: 20px;
            //                                  margin: 20px 0;
            //                                  border-radius: 4px;
            //                                }}
            //                                .button-container {{
            //                                  text-align: center;
            //                                  margin: 30px 0;
            //                                }}
            //                                .button {{
            //                                  background-color: #1ea4aa;
            //                                  color: #ffffff;
            //                                  padding: 12px 25px;
            //                                  text-decoration: none;
            //                                  border-radius: 4px;
            //                                  font-weight: bold;
            //                                }}
            //                                .blue-text {{
            //                                  color: blue;
            //                                }}
            //                              </style>
            //                            </head>
            //                            <body>
            //                              <div class='email-container'>
            //                                <div class='header'>
            //                                  Community Smart Access
            //                                </div>
            //                                <div class='content'>
            //                                  <h2>Hello {0},</h2>
            //                                  <p>We've received a password reset request for your account. If this wasn't you, please ignore this message or contact support.</p>
            //                                  <div class='notification-box'>
            //                                    <strong>Username:</strong>
            //                                    <p class='blue-text'>{1}</p>
            //                                    <strong>Temporary Password:</strong>
            //                                    <p class='blue-text'>{2}</p>
            //                                  </div>
            //                                  <p>To reset your password, click 'Go to Resident Portal' button and enter your username and temporary password as provided.</p>
            //                                  <p>Once logged in, you will be prompted to change your password.</p>
            //                                  <div class='button-container'>
            //                                    <a href='{portal_link}' class='button'>Go to Resident Portal</a>
            //                                  </div>
            //                                  <div class='footer'>
            //                                    <p>If you have any questions or need assistance, please don't hesitate to contact our support team.</p>
            //                                    <strong>Best regards,</strong>
            //                                    <p>The Team at Community Smart Access</p>
            //                                  </div>
            //                                </div>
            //                              </div>
            //                            </body>
            //                            </html>";








            string htmlTemplate = @"
                                <!DOCTYPE html>
                                <html lang='en'>
                                <head>
                                  <meta charset='UTF-8' />
                                  <meta name='viewport' content='width=device-width, initial-scale=1.0' />
                                  <title>FPMSB PROCURA - Account Created</title>
                                  <style>
                                    body {{
                                      font-family: Arial, Helvetica, sans-serif;
                                      background-color: #eef1f7;
                                      margin: 0;
                                      padding: 0;
                                      color: #2b2b2b;
                                    }}
                                    .email-container {{
                                      max-width: 640px;
                                      margin: 0 auto;
                                      background-color: #eef1f7;
                                      border-radius: 6px;
                                      overflow: hidden;
                                    }}
                                    .header {{
                                      background-color: #B84521;
                                      color: #ffffff;
                                      padding: 28px 20px;
                                      text-align: center;
                                    }}
                                    .header .title {{
                                      font-size: 22px;
                                      font-weight: bold;
                                      margin-bottom: 10px;
                                    }}
                                    .header .subtitle {{
                                      font-size: 18px;
                                      font-weight: 600;
                                    }}
                                    .content {{
                                      padding: 28px 32px;
                                      background-color: #eef1f7;
                                    }}
                                    .content p {{
                                      line-height: 1.5;
                                      margin: 10px 0;
                                    }}
                                    .credentials {{
                                      margin: 20px 0;
                                    }}
                                    .credentials table {{
                                      border-collapse: collapse;
                                    }}
                                    .credentials td {{
                                      padding: 4px 10px 4px 0;
                                      vertical-align: top;
                                    }}
                                    .credentials .label {{
                                      font-weight: bold;
                                    }}
                                    .button-container {{
                                      text-align: center;
                                      margin: 28px 0;
                                    }}
                                    .button {{
                                      background-color: #B84521;
                                      color: #ffffff;
                                      padding: 14px 28px;
                                      text-decoration: none;
                                      border-radius: 4px;
                                      font-weight: bold;
                                      display: inline-block;
                                    }}
                                    ol.steps {{
                                      padding-left: 20px;
                                      margin: 10px 0 0 0;
                                    }}
                                    ol.steps li {{
                                      margin-bottom: 6px;
                                      line-height: 1.5;
                                    }}
                                    .footer {{
                                      background-color: #e4e7ed;
                                      text-align: center;
                                      padding: 18px 20px;
                                      font-size: 13px;
                                      color: #6b6b6b;
                                    }}
                                    .footer p {{
                                      margin: 4px 0;
                                    }}
                                  </style>
                                </head>
                                <body>
                                  <div class='email-container'>
                                    <div class='header'>
                                      <div class='title'>FPMSB PROCURA</div>
                                      <div class='subtitle'>Felda Plantation Management Sdn. Bhd.</div>
                                    </div>
                                    <div class='content'>
                                      <p><strong>Dear {0},</strong></p>
                                      <p>Your account for the FPMSB PROCURA system has been successfully created by the system administrator.</p>
                                      <div class='credentials'>
                                        <table>
                                          <tr>
                                            <td class='label'>Staff ID</td>
                                            <td>:</td>
                                            <td>{1}</td>
                                          </tr>
                                          <tr>
                                            <td class='label'>Temporary Password</td>
                                            <td>:</td>
                                            <td>{2}</td>
                                          </tr>
                                        </table>
                                      </div>
                                      <div class='button-container'>
                                        <a href='{3}' class='button' style='color:#ffffff;'>Login to FPMSB PROCURA</a>
                                      </div>
                                      <ol class='steps'>
                                        <li>Click the button above to go to the login page.</li>
                                        <li>Enter your <strong>Staff ID</strong> and <strong>Temporary Password</strong>.</li>
                                        <li>The system will immediately prompt you to <strong>set a new password</strong>.</li>
                                        <li>Once updated, you will have full access to your dashboard.</li>
                                      </ol>
                                    </div>
                                    <div class='footer'>
                                      <p>&copy; 2026 FPMSB. All rights reserved.</p>
                                      <p>This is an automated system message. Please do not reply directly to this email.</p>
                                      <p>Technical Support: support@fpmsb.com.my</p>
                                    </div>
                                  </div>
                                </body>
                                </html>";


            return string.Format(htmlTemplate,
                     residentFullName,
                     residentEmail,
                     randomPassword,
                     residentPageUrl);
        }
        public static string GenerateRandomPassword(int length = 10)
        {
            const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnpqrstuvwxyz23456789!@$?";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, length)
                                        .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public static string GetRenewalSuccessEmailBody(string companyName, string vendorCode, string renewedOn, string newExpiryDate, string loginUrl)
        {
            string htmlTemplate = @"
                                <!DOCTYPE html>
                                <html lang='en'>
                                <head>
                                  <meta charset='UTF-8' />
                                  <meta name='viewport' content='width=device-width, initial-scale=1.0' />
                                  <title>FPMSB PROCURA - Renewal Successful</title>
                                  <style>
                                    body {{
                                      font-family: Arial, Helvetica, sans-serif;
                                      background-color: #eef1f7;
                                      margin: 0;
                                      padding: 0;
                                      color: #2b2b2b;
                                    }}
                                    .email-container {{
                                      max-width: 640px;
                                      margin: 0 auto;
                                      background-color: #eef1f7;
                                      border-radius: 6px;
                                      overflow: hidden;
                                    }}
                                    .header {{
                                      background-color: #B84521;
                                      color: #ffffff;
                                      padding: 28px 20px;
                                      text-align: center;
                                    }}
                                    .header .title {{
                                      font-size: 22px;
                                      font-weight: bold;
                                      margin-bottom: 10px;
                                    }}
                                    .header .subtitle {{
                                      font-size: 18px;
                                      font-weight: 600;
                                    }}
                                    .content {{
                                      padding: 28px 32px;
                                      background-color: #eef1f7;
                                    }}
                                    .content p {{
                                      line-height: 1.5;
                                      margin: 10px 0;
                                    }}
                                    .credentials {{
                                      margin: 20px 0;
                                    }}
                                    .credentials table {{
                                      border-collapse: collapse;
                                    }}
                                    .credentials td {{
                                      padding: 4px 10px 4px 0;
                                      vertical-align: top;
                                    }}
                                    .credentials .label {{
                                      font-weight: bold;
                                    }}
                                    .confirm-box {{
                                      background-color: #f0f7ef;
                                      border-left: 4px solid #2E7D32;
                                      padding: 14px 18px;
                                      margin: 18px 0;
                                      border-radius: 4px;
                                      font-weight: 600;
                                      color: #1e4620;
                                    }}
                                    .button-container {{
                                      text-align: center;
                                      margin: 28px 0;
                                    }}
                                    .button {{
                                      background-color: #B84521;
                                      color: #ffffff;
                                      padding: 14px 28px;
                                      text-decoration: none;
                                      border-radius: 4px;
                                      font-weight: bold;
                                      display: inline-block;
                                    }}
                                    .footer {{
                                      background-color: #e4e7ed;
                                      text-align: center;
                                      padding: 18px 20px;
                                      font-size: 13px;
                                      color: #6b6b6b;
                                    }}
                                    .footer p {{
                                      margin: 4px 0;
                                    }}
                                  </style>
                                </head>
                                <body>
                                  <div class='email-container'>
                                    <div class='header'>
                                      <div class='title'>FPMSB PROCURA</div>
                                      <div class='subtitle'>Felda Plantation Management Sdn. Bhd.</div>
                                    </div>
                                    <div class='content'>
                                      <p><strong>Dear {0},</strong></p>
                                      <div class='confirm-box'>Your vendor registration has been successfully renewed.</div>
                                      <p>Thank you for renewing your FPMSB PROCURA vendor registration. Your account remains active and you can continue participating in tenders without interruption.</p>
                                      <div class='credentials'>
                                        <table>
                                          <tr>
                                            <td class='label'>Vendor Code</td>
                                            <td>:</td>
                                            <td>{1}</td>
                                          </tr>
                                          <tr>
                                            <td class='label'>Renewed On</td>
                                            <td>:</td>
                                            <td>{2}</td>
                                          </tr>
                                          <tr>
                                            <td class='label'>New Expiry Date</td>
                                            <td>:</td>
                                            <td>{3}</td>
                                          </tr>
                                        </table>
                                      </div>
                                      <div class='button-container'>
                                        <a href='{4}' class='button' style='color:#ffffff;'>Go to Vendor Portal</a>
                                      </div>
                                    </div>
                                    <div class='footer'>
                                      <p>&copy; 2026 FPMSB. All rights reserved.</p>
                                      <p>This is an automated system message. Please do not reply directly to this email.</p>
                                      <p>Technical Support: support@fpmsb.com.my</p>
                                    </div>
                                  </div>
                                </body>
                                </html>";

            return string.Format(htmlTemplate,
                                 companyName ?? string.Empty,
                                 string.IsNullOrWhiteSpace(vendorCode) ? "—" : vendorCode,
                                 renewedOn ?? string.Empty,
                                 newExpiryDate ?? string.Empty,
                                 loginUrl ?? string.Empty);
        }

        public static string GetRenewalReminderEmailBody(string companyName, string vendorCode, string expiryDate, int daysRemaining, string renewalUrl)
        {
            var urgency = daysRemaining <= 7 ? "#C0392B" : "#B84521";
            var headline = daysRemaining == 1
                ? "Your vendor registration expires tomorrow"
                : $"Your vendor registration expires in {daysRemaining} day{(daysRemaining == 1 ? string.Empty : "s")}";

            string htmlTemplate = @"
                                <!DOCTYPE html>
                                <html lang='en'>
                                <head>
                                  <meta charset='UTF-8' />
                                  <meta name='viewport' content='width=device-width, initial-scale=1.0' />
                                  <title>FPMSB PROCURA - Renewal Reminder</title>
                                  <style>
                                    body {{
                                      font-family: Arial, Helvetica, sans-serif;
                                      background-color: #eef1f7;
                                      margin: 0;
                                      padding: 0;
                                      color: #2b2b2b;
                                    }}
                                    .email-container {{
                                      max-width: 640px;
                                      margin: 0 auto;
                                      background-color: #eef1f7;
                                      border-radius: 6px;
                                      overflow: hidden;
                                    }}
                                    .header {{
                                      background-color: {5};
                                      color: #ffffff;
                                      padding: 28px 20px;
                                      text-align: center;
                                    }}
                                    .header .title {{
                                      font-size: 22px;
                                      font-weight: bold;
                                      margin-bottom: 10px;
                                    }}
                                    .header .subtitle {{
                                      font-size: 18px;
                                      font-weight: 600;
                                    }}
                                    .content {{
                                      padding: 28px 32px;
                                      background-color: #eef1f7;
                                    }}
                                    .content p {{
                                      line-height: 1.5;
                                      margin: 10px 0;
                                    }}
                                    .credentials {{
                                      margin: 20px 0;
                                    }}
                                    .credentials table {{
                                      border-collapse: collapse;
                                    }}
                                    .credentials td {{
                                      padding: 4px 10px 4px 0;
                                      vertical-align: top;
                                    }}
                                    .credentials .label {{
                                      font-weight: bold;
                                    }}
                                    .alert {{
                                      background-color: #fff5f1;
                                      border-left: 4px solid {5};
                                      padding: 14px 18px;
                                      margin: 18px 0;
                                      border-radius: 4px;
                                      font-weight: 600;
                                    }}
                                    .button-container {{
                                      text-align: center;
                                      margin: 28px 0;
                                    }}
                                    .button {{
                                      background-color: {5};
                                      color: #ffffff;
                                      padding: 14px 28px;
                                      text-decoration: none;
                                      border-radius: 4px;
                                      font-weight: bold;
                                      display: inline-block;
                                    }}
                                    .footer {{
                                      background-color: #e4e7ed;
                                      text-align: center;
                                      padding: 18px 20px;
                                      font-size: 13px;
                                      color: #6b6b6b;
                                    }}
                                    .footer p {{
                                      margin: 4px 0;
                                    }}
                                  </style>
                                </head>
                                <body>
                                  <div class='email-container'>
                                    <div class='header'>
                                      <div class='title'>FPMSB PROCURA</div>
                                      <div class='subtitle'>Felda Plantation Management Sdn. Bhd.</div>
                                    </div>
                                    <div class='content'>
                                      <p><strong>Dear {0},</strong></p>
                                      <div class='alert'>{6}</div>
                                      <p>Your vendor registration with FPMSB PROCURA is approaching its expiry date. Renew now to continue participating in tenders without interruption.</p>
                                      <div class='credentials'>
                                        <table>
                                          <tr>
                                            <td class='label'>Vendor Code</td>
                                            <td>:</td>
                                            <td>{1}</td>
                                          </tr>
                                          <tr>
                                            <td class='label'>Expiry Date</td>
                                            <td>:</td>
                                            <td>{2}</td>
                                          </tr>
                                          <tr>
                                            <td class='label'>Days Remaining</td>
                                            <td>:</td>
                                            <td>{3}</td>
                                          </tr>
                                        </table>
                                      </div>
                                      <div class='button-container'>
                                        <a href='{4}' class='button' style='color:#ffffff;'>Renew Registration Now</a>
                                      </div>
                                      <p>If you do not renew before the expiry date, your account status will change to <strong>Expired</strong> and you will not be able to submit new bids until renewal is completed.</p>
                                    </div>
                                    <div class='footer'>
                                      <p>&copy; 2026 FPMSB. All rights reserved.</p>
                                      <p>This is an automated system message. Please do not reply directly to this email.</p>
                                      <p>Technical Support: support@fpmsb.com.my</p>
                                    </div>
                                  </div>
                                </body>
                                </html>";

            return string.Format(htmlTemplate,
                                 companyName ?? string.Empty,
                                 string.IsNullOrWhiteSpace(vendorCode) ? "—" : vendorCode,
                                 expiryDate ?? string.Empty,
                                 daysRemaining,
                                 renewalUrl ?? string.Empty,
                                 urgency,
                                 headline);
        }

        public static string GetCategoryCodeApprovalEmailBody(string companyName, string vendorCode, string approvedDate, string categoryListHtml, string loginUrl)
        {
            string htmlTemplate = @"
                                <!DOCTYPE html>
                                <html lang='en'>
                                <head>
                                  <meta charset='UTF-8' />
                                  <meta name='viewport' content='width=device-width, initial-scale=1.0' />
                                  <title>FPMSB PROCURA - Category Code Updated</title>
                                  <style>
                                    body {{
                                      font-family: Arial, Helvetica, sans-serif;
                                      background-color: #eef1f7;
                                      margin: 0;
                                      padding: 0;
                                      color: #2b2b2b;
                                    }}
                                    .email-container {{
                                      max-width: 640px;
                                      margin: 0 auto;
                                      background-color: #eef1f7;
                                      border-radius: 6px;
                                      overflow: hidden;
                                    }}
                                    .header {{
                                      background-color: #B84521;
                                      color: #ffffff;
                                      padding: 28px 20px;
                                      text-align: center;
                                    }}
                                    .header .title {{
                                      font-size: 22px;
                                      font-weight: bold;
                                      margin-bottom: 10px;
                                    }}
                                    .header .subtitle {{
                                      font-size: 18px;
                                      font-weight: 600;
                                    }}
                                    .content {{
                                      padding: 28px 32px;
                                      background-color: #eef1f7;
                                    }}
                                    .content p {{
                                      line-height: 1.5;
                                      margin: 10px 0;
                                    }}
                                    .credentials {{
                                      margin: 20px 0;
                                    }}
                                    .credentials table {{
                                      border-collapse: collapse;
                                    }}
                                    .credentials td {{
                                      padding: 4px 10px 4px 0;
                                      vertical-align: top;
                                    }}
                                    .credentials .label {{
                                      font-weight: bold;
                                    }}
                                    .category-box {{
                                      background-color: #ffffff;
                                      border: 1px solid #d9dde5;
                                      border-radius: 6px;
                                      padding: 16px 20px;
                                      margin: 16px 0;
                                    }}
                                    .category-box ul {{
                                      margin: 8px 0 0 0;
                                      padding-left: 20px;
                                    }}
                                    .category-box li {{
                                      margin-bottom: 6px;
                                      line-height: 1.5;
                                    }}
                                    .button-container {{
                                      text-align: center;
                                      margin: 28px 0;
                                    }}
                                    .button {{
                                      background-color: #B84521;
                                      color: #ffffff;
                                      padding: 14px 28px;
                                      text-decoration: none;
                                      border-radius: 4px;
                                      font-weight: bold;
                                      display: inline-block;
                                    }}
                                    .footer {{
                                      background-color: #e4e7ed;
                                      text-align: center;
                                      padding: 18px 20px;
                                      font-size: 13px;
                                      color: #6b6b6b;
                                    }}
                                    .footer p {{
                                      margin: 4px 0;
                                    }}
                                  </style>
                                </head>
                                <body>
                                  <div class='email-container'>
                                    <div class='header'>
                                      <div class='title'>FPMSB PROCURA</div>
                                      <div class='subtitle'>Felda Plantation Management Sdn. Bhd.</div>
                                    </div>
                                    <div class='content'>
                                      <p><strong>Dear {0},</strong></p>
                                      <p>Your category code change request has been <strong>approved</strong>. The updated categories are now linked to your vendor profile and can be used when applying for tenders.</p>
                                      <div class='credentials'>
                                        <table>
                                          <tr>
                                            <td class='label'>Vendor Code</td>
                                            <td>:</td>
                                            <td>{1}</td>
                                          </tr>
                                          <tr>
                                            <td class='label'>Approved On</td>
                                            <td>:</td>
                                            <td>{2}</td>
                                          </tr>
                                        </table>
                                      </div>
                                      <div class='category-box'>
                                        <strong>Approved categories:</strong>
                                        <ul>
                                          {3}
                                        </ul>
                                      </div>
                                      <div class='button-container'>
                                        <a href='{4}' class='button' style='color:#ffffff;'>View My Vendor Profile</a>
                                      </div>
                                    </div>
                                    <div class='footer'>
                                      <p>&copy; 2026 FPMSB. All rights reserved.</p>
                                      <p>This is an automated system message. Please do not reply directly to this email.</p>
                                      <p>Technical Support: support@fpmsb.com.my</p>
                                    </div>
                                  </div>
                                </body>
                                </html>";

            return string.Format(htmlTemplate,
                                 companyName ?? string.Empty,
                                 string.IsNullOrWhiteSpace(vendorCode) ? "—" : vendorCode,
                                 approvedDate ?? string.Empty,
                                 string.IsNullOrWhiteSpace(categoryListHtml) ? "<li>(no items)</li>" : categoryListHtml,
                                 loginUrl ?? string.Empty);
        }

        public static string GetVendorApprovalEmailBody(string companyName, string vendorCode, string approvedDate, string expiryDate, string loginUrl)
        {
            string htmlTemplate = @"
                                <!DOCTYPE html>
                                <html lang='en'>
                                <head>
                                  <meta charset='UTF-8' />
                                  <meta name='viewport' content='width=device-width, initial-scale=1.0' />
                                  <title>FPMSB PROCURA - Registration Approved</title>
                                  <style>
                                    body {{
                                      font-family: Arial, Helvetica, sans-serif;
                                      background-color: #eef1f7;
                                      margin: 0;
                                      padding: 0;
                                      color: #2b2b2b;
                                    }}
                                    .email-container {{
                                      max-width: 640px;
                                      margin: 0 auto;
                                      background-color: #eef1f7;
                                      border-radius: 6px;
                                      overflow: hidden;
                                    }}
                                    .header {{
                                      background-color: #B84521;
                                      color: #ffffff;
                                      padding: 28px 20px;
                                      text-align: center;
                                    }}
                                    .header .title {{
                                      font-size: 22px;
                                      font-weight: bold;
                                      margin-bottom: 10px;
                                    }}
                                    .header .subtitle {{
                                      font-size: 18px;
                                      font-weight: 600;
                                    }}
                                    .content {{
                                      padding: 28px 32px;
                                      background-color: #eef1f7;
                                    }}
                                    .content p {{
                                      line-height: 1.5;
                                      margin: 10px 0;
                                    }}
                                    .credentials {{
                                      margin: 20px 0;
                                    }}
                                    .credentials table {{
                                      border-collapse: collapse;
                                    }}
                                    .credentials td {{
                                      padding: 4px 10px 4px 0;
                                      vertical-align: top;
                                    }}
                                    .credentials .label {{
                                      font-weight: bold;
                                    }}
                                    .button-container {{
                                      text-align: center;
                                      margin: 28px 0;
                                    }}
                                    .button {{
                                      background-color: #B84521;
                                      color: #ffffff;
                                      padding: 14px 28px;
                                      text-decoration: none;
                                      border-radius: 4px;
                                      font-weight: bold;
                                      display: inline-block;
                                    }}
                                    .footer {{
                                      background-color: #e4e7ed;
                                      text-align: center;
                                      padding: 18px 20px;
                                      font-size: 13px;
                                      color: #6b6b6b;
                                    }}
                                    .footer p {{
                                      margin: 4px 0;
                                    }}
                                  </style>
                                </head>
                                <body>
                                  <div class='email-container'>
                                    <div class='header'>
                                      <div class='title'>FPMSB PROCURA</div>
                                      <div class='subtitle'>Felda Plantation Management Sdn. Bhd.</div>
                                    </div>
                                    <div class='content'>
                                      <p><strong>Dear {0},</strong></p>
                                      <p>We are pleased to inform you that your vendor registration for FPMSB PROCURA has been <strong>approved</strong>. Your Vendor Code has been issued and your account is now active.</p>
                                      <div class='credentials'>
                                        <table>
                                          <tr>
                                            <td class='label'>Vendor Code</td>
                                            <td>:</td>
                                            <td>{1}</td>
                                          </tr>
                                          <tr>
                                            <td class='label'>Approved On</td>
                                            <td>:</td>
                                            <td>{2}</td>
                                          </tr>
                                          <tr>
                                            <td class='label'>Registration Valid Until</td>
                                            <td>:</td>
                                            <td>{3}</td>
                                          </tr>
                                        </table>
                                      </div>
                                      <p>You can now participate in tenders and manage your vendor profile through the portal. Please note that your registration is valid for 3 years from the approval date; renewal reminders will be sent as the expiry date approaches.</p>
                                      <div class='button-container'>
                                        <a href='{4}' class='button' style='color:#ffffff;'>Go to Vendor Portal</a>
                                      </div>
                                    </div>
                                    <div class='footer'>
                                      <p>&copy; 2026 FPMSB. All rights reserved.</p>
                                      <p>This is an automated system message. Please do not reply directly to this email.</p>
                                      <p>Technical Support: support@fpmsb.com.my</p>
                                    </div>
                                  </div>
                                </body>
                                </html>";

            return string.Format(htmlTemplate,
                                 companyName ?? string.Empty,
                                 vendorCode ?? string.Empty,
                                 approvedDate ?? string.Empty,
                                 expiryDate ?? string.Empty,
                                 loginUrl ?? string.Empty);
        }

        public static string GetVendorRegistrationEmailBody(string companyName, string rocNumber, string loginUrl)
        {
            string htmlTemplate = @"
                                <!DOCTYPE html>
                                <html lang='en'>
                                <head>
                                  <meta charset='UTF-8' />
                                  <meta name='viewport' content='width=device-width, initial-scale=1.0' />
                                  <title>FPMSB PROCURA - Registration Received</title>
                                  <style>
                                    body {{
                                      font-family: Arial, Helvetica, sans-serif;
                                      background-color: #eef1f7;
                                      margin: 0;
                                      padding: 0;
                                      color: #2b2b2b;
                                    }}
                                    .email-container {{
                                      max-width: 640px;
                                      margin: 0 auto;
                                      background-color: #eef1f7;
                                      border-radius: 6px;
                                      overflow: hidden;
                                    }}
                                    .header {{
                                      background-color: #B84521;
                                      color: #ffffff;
                                      padding: 28px 20px;
                                      text-align: center;
                                    }}
                                    .header .title {{
                                      font-size: 22px;
                                      font-weight: bold;
                                      margin-bottom: 10px;
                                    }}
                                    .header .subtitle {{
                                      font-size: 18px;
                                      font-weight: 600;
                                    }}
                                    .content {{
                                      padding: 28px 32px;
                                      background-color: #eef1f7;
                                    }}
                                    .content p {{
                                      line-height: 1.5;
                                      margin: 10px 0;
                                    }}
                                    .credentials {{
                                      margin: 20px 0;
                                    }}
                                    .credentials table {{
                                      border-collapse: collapse;
                                    }}
                                    .credentials td {{
                                      padding: 4px 10px 4px 0;
                                      vertical-align: top;
                                    }}
                                    .credentials .label {{
                                      font-weight: bold;
                                    }}
                                    .button-container {{
                                      text-align: center;
                                      margin: 28px 0;
                                    }}
                                    .button {{
                                      background-color: #B84521;
                                      color: #ffffff;
                                      padding: 14px 28px;
                                      text-decoration: none;
                                      border-radius: 4px;
                                      font-weight: bold;
                                      display: inline-block;
                                    }}
                                    .footer {{
                                      background-color: #e4e7ed;
                                      text-align: center;
                                      padding: 18px 20px;
                                      font-size: 13px;
                                      color: #6b6b6b;
                                    }}
                                    .footer p {{
                                      margin: 4px 0;
                                    }}
                                  </style>
                                </head>
                                <body>
                                  <div class='email-container'>
                                    <div class='header'>
                                      <div class='title'>FPMSB PROCURA</div>
                                      <div class='subtitle'>Felda Plantation Management Sdn. Bhd.</div>
                                    </div>
                                    <div class='content'>
                                      <p><strong>Dear {0},</strong></p>
                                      <p>Thank you for registering as a vendor in FPMSB PROCURA. Your registration has been received and is now under review.</p>
                                      <div class='credentials'>
                                        <table>
                                          <tr>
                                            <td class='label'>Company Registration No.</td>
                                            <td>:</td>
                                            <td>{1}</td>
                                          </tr>
                                          <tr>
                                            <td class='label'>Submitted On</td>
                                            <td>:</td>
                                            <td>{2}</td>
                                          </tr>
                                        </table>
                                      </div>
                                      <p>You will receive a follow-up email once your registration has been approved and your Vendor Code has been issued.</p>
                                      <div class='button-container'>
                                        <a href='{3}' class='button' style='color:#ffffff;'>Login to Track Status</a>
                                      </div>
                                    </div>
                                    <div class='footer'>
                                      <p>&copy; 2026 FPMSB. All rights reserved.</p>
                                      <p>This is an automated system message. Please do not reply directly to this email.</p>
                                      <p>Technical Support: support@fpmsb.com.my</p>
                                    </div>
                                  </div>
                                </body>
                                </html>";

            return string.Format(htmlTemplate,
                                 companyName ?? string.Empty,
                                 rocNumber ?? string.Empty,
                                 DateTime.UtcNow.ToString("dd MMM yyyy"),
                                 loginUrl ?? string.Empty);
        }

        public static string GetResetPasswordEmailBody(string residentFullName, string residentEmail, string randomPassword, string residentPageUrl, string community)
        {
            //return $@"
            //    <html>
            //      <body>
            //        <p>Dear {residentFullName},</p>
            //        <p>We received a request to reset the password associated with your account.</p>
            //        <p><strong>Please find your password reset details below:</strong></p>
            //        <p>
            //          <b>System URL</b>: <a href='{residentPageUrl}'>{residentPageUrl}</a><br/>
            //          <b>Username</b>: {residentEmail}<br/>
            //          <b>Temporary Password</b>: {randomPassword}<br/>
            //          <b>Community</b>: {community}
            //        </p>
            //        <p>
            //        Please click system url link and enter your username and temporary password to access Resident portal</p>
            //        <p>
            //          For security reasons, you will be prompted to change your password upon your next login.
            //        </p>
            //        <p>If you did not request a password reset, please contact the system administrator immediately.</p>
            //        <p>Best regards,<br/>CSA Team</p>
            //      </body>
            //    </html>";

            string htmlTemplate = @"
                                        <!DOCTYPE html>
                                        <html lang='en'>
                                        <head>
                                          <meta charset='UTF-8' />
                                          <meta name='viewport' content='width=device-width, initial-scale=1.0' />
                                          <title>Email Notification</title>
                                          <style>
                                            body {{
                                              font-family: Roboto, sans-serif;
                                              background-color: #f5f7fa;
                                              margin: 0;
                                              padding: 0;
                                            }}
                                            .email-container {{
                                              max-width: 600px;
                                              margin: 0 auto;
                                              background-color: #ffffff;
                                              border-radius: 6px;
                                              overflow: hidden;
                                            }}
                                            .header {{
                                              background-color: #1ea4aa;
                                              color: #ffffff;
                                              padding: 20px;
                                              text-align: center;
                                              font-size: 20px;
                                              font-weight: bold;
                                            }}
                                            .content {{
                                              padding: 30px;
                                            }}
                                            .content h2 {{
                                              margin-top: 0;
                                              color: #2b2b2b;
                                            }}
                                            .content p, .notification-box p, .footer p {{
                                              line-height: 1.5;
                                            }}
                                            .notification-box {{
                                              background-color: #f1f3f6;
                                              border-left: 4px solid #1ea4aa;
                                              padding: 20px;
                                              margin: 20px 0;
                                              border-radius: 4px;
                                            }}
                                            .button-container {{
                                              text-align: center;
                                              margin: 30px 0;
                                            }}
                                            .button {{
                                              background-color: #1ea4aa;
                                              color: #ffffff;
                                              padding: 12px 25px;
                                              text-decoration: none;
                                              border-radius: 4px;
                                              font-weight: bold;
                                            }}
                                            .blue-text {{
                                              color: blue;
                                            }}
                                          </style>
                                        </head>
                                        <body>
                                          <div class='email-container'>
                                            <div class='header'>
                                              Community Smart Access
                                            </div>
                                            <div class='content'>
                                              <h2>Hello {0},</h2>
                                              <p>We've received a password reset request for your account. If this wasn't you, please ignore this message or contact support.</p>
                                              <div class='notification-box'>
                                                <strong>Username:</strong>
                                                <p class='blue-text'>{1}</p>
                                                <strong>Temporary Password:</strong>
                                                <p class='blue-text'>{2}</p>
                                              </div>
                                              <p>To reset your password, click 'Go to Resident Portal' button and enter your username and temporary password as provided.</p>
                                              <p>Once logged in, you will be prompted to change your password.</p>
                                              <div class='button-container'>
                                                <a href='{3}' class='button' style='color:white;'>Go to Resident Portal</a>
                                              </div>
                                              <div class='footer'>
                                                <p>If you have any questions or need assistance, please don't hesitate to contact our support team.</p>
                                                <strong>Best regards,</strong>
                                                <p>The Team at Community Smart Access</p>
                                              </div>
                                            </div>
                                          </div>
                                        </body>
                                        </html>";
            return string.Format(htmlTemplate,
                   residentFullName,
                   
                   residentEmail,
                   randomPassword,
                   residentPageUrl);

        }
        public static string GetQrEmailBody(string residentFullName, string residentEmail, string residentPageUrl, string community)
        {
            return $@"
                <html>
                  <body>
                    <p>Dear {residentFullName},</p>
                    <p>Please find the QR as attched.</p>
                    <p>
                      <b>System URL</b>: <a href='{residentPageUrl}'>{residentPageUrl}</a><br/>
                      <b>Username</b>: {residentEmail}<br/>
                      <b>Community</b>: {community}
                    </p>
                    <p>Best regards,<br/>CSA Team</p>
                  </body>
                </html>";
        }
        //public static string ComplaintEmailBody(ComplaintDetail dto)
        //{
        //    return $@"
        //        <html>
        //          <body>
        //            <p>Dear {dto.Resident?.Name},</p>
        //            <p>Your complaint has been sent to the officer on-duty and you will be contacted shortly</p>
        //            <p>
        //              <b>Complaint Type:</b> {dto.ComplaintType?.Name}<br/>
        //              <b>Complaint Details:</b> {dto.Description}<br/>
        //               <b>Date / Time:</b>: {dto.ComplaintDate?.ToString("dd/MM/yyyy HH:mm")}
        //            </p>
        //            <p>Best regards,<br/>CSA Team</p>
        //          </body>
        //        </html>";
        //}
        //public static string ComplaintEmailSubmissionBody(ComplaintDetail dto, string communityName)
        //{
        //    string htmlTemplate = @"
        //                        <!DOCTYPE html>
        //                        <html lang='en'>
        //                        <head>
        //                          <meta charset='UTF-8' />
        //                          <meta name='viewport' content='width=device-width, initial-scale=1.0' />
        //                          <title>Email Notification</title>
        //                          <style>
        //                            body {{
        //                              font-family: Roboto, sans-serif;
        //                              background-color: #f5f7fa;
        //                              margin: 0;
        //                              padding: 0;
        //                            }}
        //                            .email-container {{
        //                              max-width: 600px;
        //                              margin: 0 auto;
        //                              background-color: #ffffff;
        //                              border-radius: 6px;
        //                              overflow: hidden;
        //                            }}
        //                            .header {{
        //                              background-color: #1ea4aa;
        //                              color: #ffffff;
        //                              padding: 20px;
        //                              text-align: center;
        //                              font-size: 20px;
        //                              font-weight: bold;
        //                            }}
        //                            .content {{
        //                              padding: 30px;
        //                            }}
        //                            .content h2 {{
        //                              margin-top: 0;
        //                              color: #2b2b2b;
        //                            }}
        //                            .content p, .notification-box p, .footer p {{
        //                              line-height: 1.5;
        //                            }}
        //                            .notification-box {{
        //                              background-color: #f1f3f6;
        //                              border-left: 4px solid #1ea4aa;
        //                              padding: 20px;
        //                              margin: 20px 0;
        //                              border-radius: 4px;
        //                            }}
        //                            .button-container {{
        //                              text-align: center;
        //                              margin: 30px 0;
        //                            }}
        //                            .button {{
        //                              background-color: #1ea4aa;
        //                              color: white;
        //                              padding: 12px 25px;
        //                              text-decoration: none;
        //                              border-radius: 4px;
        //                              font-weight: bold;
        //                            }}
        //                            .blue-text {{
        //                              color: blue;
        //                            }}
        //                            .italic {{
        //                              font-style: italic;
        //                              color: blue;
        //                            }}
        //                          </style>
        //                        </head>
        //                        <body>
        //                          <div class='email-container'>
        //                            <div class='header'>
        //                              Community Smart Access
        //                            </div>
        //                            <div class='content'>
        //                              <h2>Hello {0},</h2>
        //                              <p>We have received your complaint and have sent it to the officer on-duty. We will update the progress to you accordingly via email.</p>
        //                              <div class='notification-box'>
        //                                <strong>Reference No:</strong>
        //                                <p class='blue-text'>{1}</p>
        //                                <strong>Type:</strong>
        //                                <p class='blue-text'>{2}</p>
        //                                <strong>Complaint Date / Time:</strong>
        //                                <p class='blue-text'>{3}</p>
        //                                <strong>Status:</strong>
        //                                <p class='blue-text'>{4}</p>
        //                                <strong>Complaint Details:</strong>
        //                                <p class='italic'>“{5}”</p>
        //                              </div>
        //                              <p>If you have any questions or need assistance, please don't hesitate to contact our support team.</p>
        //                              <strong>Best regards,</strong>
        //                              <p>The Team at Community Smart Access</p>
        //                            </div>
        //                          </div>
        //                        </body>
        //                        </html>";
        //    return string.Format(htmlTemplate,
        //                                        dto.Resident?.Name,
        //                                        dto.ComplainRefNo,
        //                                        dto.ComplaintType?.Name,
        //                                        dto.ComplaintDate?.ToString("dd/MM/yyyy HH:mm"),
        //                                        "Submitted",
        //                                        dto.Description
        //                                    );
         
        //}
        //public static string ComplaintStatusUpdateEmailBody(ComplaintDetail dto, string communityName)
        //{
            
        //    string htmlTemplate = @"
        //                            <!DOCTYPE html>
        //                            <html lang='en'>
        //                            <head>
        //                              <meta charset='UTF-8' />
        //                              <meta name='viewport' content='width=device-width, initial-scale=1.0' />
        //                              <title>Email Notification</title>
        //                              <style>
        //                                body {{
        //                                  font-family: Roboto, sans-serif;
        //                                  background-color: #f5f7fa;
        //                                  margin: 0;
        //                                  padding: 0;
        //                                }}
        //                                .email-container {{
        //                                  max-width: 600px;
        //                                  margin: 0 auto;
        //                                  background-color: #ffffff;
        //                                  border-radius: 6px;
        //                                  overflow: hidden;
        //                                }}
        //                                .header {{
        //                                  background-color: #1ea4aa;
        //                                  color: #ffffff;
        //                                  padding: 20px;
        //                                  text-align: center;
        //                                  font-size: 20px;
        //                                  font-weight: bold;
        //                                }}
        //                                .content {{
        //                                  padding: 30px;
        //                                }}
        //                                .content h2 {{
        //                                  margin-top: 0;
        //                                  color: #2b2b2b;
        //                                }}
        //                                .content p, .notification-box p, .footer p {{
        //                                  line-height: 1.5;
        //                                }}
        //                                .notification-box {{
        //                                  background-color: #f1f3f6;
        //                                  border-left: 4px solid #1ea4aa;
        //                                  padding: 20px;
        //                                  margin: 20px 0;
        //                                  border-radius: 4px;
        //                                }}
        //                                .blue-text {{
        //                                  color: blue;
        //                                }}
        //                                .italic {{
        //                                  font-style: italic;
        //                                  color: blue;
        //                                }}
        //                              </style>
        //                            </head>
        //                            <body>
        //                              <div class='email-container'>
        //                                <div class='header'>
        //                                  Community Smart Access
        //                                </div>
        //                                <div class='content'>
        //                                  <h2>Hello {0},</h2>
        //                                  <p>We have an update regarding your complaint reference <strong>{1}</strong></p>
        //                                  <div class='notification-box'>
        //                                    <strong>Complaint Details:</strong>
        //                                    <p class='italic'>“{2}”</p>
        //                                    <strong>Update Details:</strong>
        //                                    <p class='italic'>“{3}”</p>
        //                                    <strong>Update Date / Time:</strong>
        //                                    <p class='blue-text'>{4}</p>
        //                                    <strong>Status:</strong>
        //                                    <p class='blue-text'>{5}</p>
        //                                  </div>
        //                                  <p>If you have any questions or need assistance, please don't hesitate to contact our support team.</p>
        //                                  <strong>Best regards,</strong>
        //                                  <p>The Team at Community Smart Access</p>
        //                                </div>
        //                              </div>
        //                            </body>
        //                            </html>";
        //                          return string.Format(htmlTemplate,
        //                                       dto.Resident?.Name,
        //                                       dto.ComplainRefNo,
        //                                       dto.Description,
        //                                       dto.SecurityRemarks,
        //                                       dto.UpdatedDate?.ToString("dd/MM/yyyy hh:mm tt"),
        //                                       dto.ComplaintStatus?.Name
        //                                   );
        //}


        public static string VisitorEmailQR(string toEmail, string residentFullName, byte[] imageBytes, string visitFromDate, string visitToDate,string visitType)
        {
            string htmlTemplate = @"
                            <!DOCTYPE html>
                            <html lang='en'>
                            <head>
                              <meta charset='UTF-8' />
                              <meta name='viewport' content='width=device-width, initial-scale=1.0' />
                              <title>QR Email Notification</title>
                              <style>
                                body {{
                                  font-family: Roboto, sans-serif;
                                  background-color: #f5f7fa;
                                  margin: 0;
                                  padding: 0;
                                }}
                                .email-container {{
                                  max-width: 600px;
                                  margin: 0 auto;
                                  background-color: #ffffff;
                                  border-radius: 6px;
                                  overflow: hidden;
                                }}
                                .header {{
                                  background-color: #1ea4aa;
                                  color: #ffffff;
                                  padding: 20px;
                                  text-align: center;
                                  font-size: 20px;
                                  font-weight: bold;
                                }}
                                .content {{
                                  padding: 30px;
                                }}
                                .content h2 {{
                                  margin-top: 0;
                                  color: #2b2b2b;
                                }}
                                .content p, .notification-box p, .footer p {{
                                  line-height: 1.5;
                                }}
                                .notification-box {{
                                  background-color: #f1f3f6;
                                  border-left: 4px solid #1ea4aa;
                                  padding: 20px;
                                  margin: 20px 0;
                                  border-radius: 4px;
                                }}
                                .blue-text {{
                                  color: blue;
                                }}
                                .italic {{
                                  font-style: italic;
                                  color: blue;
                                }}
                              </style>
                            </head>
                            <body>
                              <div class='email-container'>
                                <div class='header'>
                                  Community Smart Access
                                </div>
                                <div class='content'>
                                  <h2>Hello {0},</h2>
                                  <p>Here is your Visitor QR in the attachment. Please download and share it with your visitor for entry. This QR is valid only on the selected date.</p>
                                  <div class='notification-box'>
                                    <strong>Visitor QR Type:</strong>
                                    <p class=''>{1} - {2}</p>
                                    <strong>Valid From - To:</strong>
                                    <p >{3} - {4}</p>
                                  </div>
                                  <strong>Best regards,</strong>
                                  <p>The Team at Community Smart Access</p>
                                </div>
                              </div>
                            </body>
                            </html>";


            return string.Format(htmlTemplate,
                                                residentFullName,
                                                "Visitor",
                                                visitType,
                                                visitFromDate,
                                                visitToDate
                                                
                                            );

        }



        public static string EventEmailQR(string toEmail, string residentFullName, byte[] imageBytes, string eventFromDate, string eventToDate, string visitType,string eventDescription)
        {
            string htmlTemplate = @"
                            <!DOCTYPE html>
                            <html lang='en'>
                            <head>
                              <meta charset='UTF-8' />
                              <meta name='viewport' content='width=device-width, initial-scale=1.0' />
                              <title>QR Email Notification</title>
                              <style>
                                body {{
                                  font-family: Roboto, sans-serif;
                                  background-color: #f5f7fa;
                                  margin: 0;
                                  padding: 0;
                                }}
                                .email-container {{
                                  max-width: 600px;
                                  margin: 0 auto;
                                  background-color: #ffffff;
                                  border-radius: 6px;
                                  overflow: hidden;
                                }}
                                .header {{
                                  background-color: #1ea4aa;
                                  color: #ffffff;
                                  padding: 20px;
                                  text-align: center;
                                  font-size: 20px;
                                  font-weight: bold;
                                }}
                                .content {{
                                  padding: 30px;
                                }}
                                .content h2 {{
                                  margin-top: 0;
                                  color: #2b2b2b;
                                }}
                                .content p, .notification-box p, .footer p {{
                                  line-height: 1.5;
                                }}
                                .notification-box {{
                                  background-color: #f1f3f6;
                                  border-left: 4px solid #1ea4aa;
                                  padding: 20px;
                                  margin: 20px 0;
                                  border-radius: 4px;
                                }}
                                .blue-text {{
                                  color: blue;
                                }}
                                .italic {{
                                  font-style: italic;
                                  color: blue;
                                }}
                              </style>
                            </head>
                            <body>
                              <div class='email-container'>
                                <div class='header'>
                                  Community Smart Access
                                </div>
                                <div class='content'>
                                  <h2>Hello {0},</h2>
                                  <p>Here is your Event QR in the attachment. Please download and share it with your guest for entry. This QR is valid only on the selected date.</p>
                                  <div class='notification-box'>
                                    <strong>Event Details:</strong>
                                    <p class=''>{1} - {2}</p>
                                    <strong>Event From - To:</strong>
                                    <p >{3} - {4}</p>
                                  </div>
                                  <strong>Best regards,</strong>
                                  <p>The Team at Community Smart Access</p>
                                </div>
                              </div>
                            </body>
                            </html>";


            return string.Format(htmlTemplate,
                                                residentFullName,
                                                "Event",
                                                eventDescription,
                                                eventFromDate,
                                                eventToDate

                                            );

        }






    }
}
