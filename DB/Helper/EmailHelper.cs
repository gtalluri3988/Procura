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
                                      color: white;
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
                                      Procura
                                    </div>
                                    <div class='content'>
                                      <h2>Hello {0},</h2>
                                      <p>Welcome to Procura <strong>{1}</strong>. Here are the details for you to login to the Procura portal.</p>
                                      <div class='notification-box'>
                                        <strong>Username:</strong>
                                        <p class='blue-text'>{2}</p>
                                        <strong>Temporary Password:</strong>
                                        <p class='blue-text'>{3}</p>
                                      </div>
                                      <p>Please click the 'Go to Procura Porta' button and enter your username and temporary password as provided.</p>
                                      <p>For security reasons, you will be prompted to change the password upon your first login.</p>
                                      <div class='button-container'>
                                        <a href='{4}' class='button' style='color:white;'>Go to Procura Portal</a>
                                      </div>
                                      <div class='footer'>
                                        <p>If you have any questions or need assistance, please don't hesitate to contact our support team.</p>
                                        <strong>Best regards,</strong>
                                        <p>The Team at Procura</p>
                                      </div>
                                    </div>
                                  </div>
                                </body>
                                </html>";


            return string.Format(htmlTemplate,
                     residentFullName,
                     community,
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
