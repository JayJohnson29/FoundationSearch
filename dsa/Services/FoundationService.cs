using System.Collections.Generic;
using System.Linq;
using dsa.Models;
using System.Data.SqlClient;
using System.Data;
using TIMSS;
using TIMSS.Client.Implementation.Security.Authentication;
using TIMSS.SqlObjects;
using System.Text;
using System;

namespace dsa.Services
{
    public class FoundationService
    {
        public class CouncilStatus
        {
            public string Member { get; set; }
            public string Accredited { get; set; }
            public string Operator { get; set; }
        }
        public List<Models.FoundationModel> GetFoundationsSql(CouncilFoundationModel model)
        {
            //new SelectListItem { Text = "N/A", Value = "0" },
            //new SelectListItem { Text = "Member", Value = "1" },
            //new SelectListItem { Text = "Accredited", Value = "2" },
            //new SelectListItem { Text = "Member and Accredited", Value = "3" },
            //new SelectListItem { Text = "Member or Accredited", Value = "4" }

            CouncilStatus status;
            switch (model.CouncilStatus)
            {
                case "0":
                    status = new CouncilStatus { Accredited = "", Member = "", Operator = "" };
                    break;
                case "1":
                    status = new CouncilStatus { Accredited = "NO", Member = "YES", Operator = "AND" };
                    break;
                case "2":
                    status = new CouncilStatus { Accredited = "YES", Member = "NO", Operator = "AND" };
                    break;
                case "3":
                    status = new CouncilStatus { Accredited = "YES", Member = "YES", Operator = "AND" };
                    break;
                case "4":
                    status = new CouncilStatus { Accredited = "YES", Member = "YES", Operator = "OR" };
                    break;
                default:
                    status = new CouncilStatus { Accredited = "", Member = "", Operator = "" };
                    break;
            }

            string congressional = string.Empty;

            if (!string.IsNullOrEmpty(model.District))
            {
                char[] arr = model.District.Where(c => (char.IsLetterOrDigit(c) || char.IsWhiteSpace(c))).ToArray();
                congressional = new string(arr);
            }


            using (var ctx = new ApplicationDbContext())
            {
                var state = new SqlParameter("@inpSTATE", model.State ?? string.Empty);
                var cong = new SqlParameter("@inpCongressional", congressional);
                var mem = new SqlParameter("@inpMEMFLAG", status.Member);
                var acc = new SqlParameter("@inpAccredited", status.Accredited);
                var opp = new SqlParameter("@inpOperator", status.Operator);

                var x = ctx.Database.SqlQuery<FoundationResponseModel>("USR_COF_CONGRESSIOAL_DISTRICT_SP @inpSTATE, @inpCongressional, @inpMEMFLAG, @inpAccredited, @inpOperator", state, cong, mem, acc, opp);
                return x.Select(a => new FoundationModel { Id = a.ORG_ID, OrgName = a.ORG_NAME, District = a.CONGRESSIONAL, CFNSB = a.ORG_COF_STATUS, CouncilStatus = a.MEMSTATUS, Address1 = a.ADDRESS_1, Address2 = a.ADDRESS_2, City = a.City, State = a.State }).ToList();
                // return x.ToList();
            }
        }

        public List<KeyValuePair<string, string>> GetAllStates1()
        {
            using (var ctx = new ApplicationDbContext())
            {
                var x = ctx.Database.SqlQuery<string>("USR_COF_CONGRESSIOAL_DISTRICT_STATES_SP");
                return x.Select(a => new KeyValuePair<string, string>(a, a)).ToList();
            }
        }
        public List<Models.FoundationModel> GetFoundations(CouncilFoundationModel model)
        {
            try
            {

                CouncilStatus status;
                switch (model.CouncilStatus)
                {
                    case "0":
                        status = new CouncilStatus { Accredited = "", Member = "", Operator = "" };
                        break;
                    case "1":
                        status = new CouncilStatus { Accredited = "NO", Member = "YES", Operator = "AND" };
                        break;
                    case "2":
                        status = new CouncilStatus { Accredited = "YES", Member = "NO", Operator = "AND" };
                        break;
                    case "3":
                        status = new CouncilStatus { Accredited = "YES", Member = "YES", Operator = "AND" };
                        break;
                    case "4":
                        status = new CouncilStatus { Accredited = "YES", Member = "YES", Operator = "OR" };
                        break;
                    default:
                        status = new CouncilStatus { Accredited = "", Member = "", Operator = "" };
                        break;
                }

                string congressional = string.Empty;

                if (!string.IsNullOrEmpty(model.District))
                {
                    char[] arr = model.District.Where(c => (char.IsLetterOrDigit(c) || char.IsWhiteSpace(c))).ToArray();
                    congressional = new string(arr);
                }


                PersonifyInitializer.Initialize(".\\", new ImmediateSeatInformationProvider());
                IStoredProcedureRequest storedProcedureRequest = new StoredProcedureRequest("USR_COF_CONGRESSIONAL_DISTRICT_SP");

                IStoredProcedureParameterItems invocationParameters = storedProcedureRequest.InvocationParameters;

                invocationParameters.Add(new StoredProcedureParameterItem("inpSTATE", model.State ?? string.Empty, ParameterDirection.Input));
                invocationParameters.Add(new StoredProcedureParameterItem("inpCongressional", congressional, ParameterDirection.Input));
                invocationParameters.Add(new StoredProcedureParameterItem("inpMEMFLAG", status.Member, ParameterDirection.Input));
                invocationParameters.Add(new StoredProcedureParameterItem("inpAccredited", status.Accredited, ParameterDirection.Input));
                invocationParameters.Add(new StoredProcedureParameterItem("inpOperator", status.Operator, ParameterDirection.Input));

                IQueryRequest request = new QueryRequest(storedProcedureRequest);
                IQueryResult queryResult = Global.App.Execute(request);


                if (!queryResult.Success || queryResult.DataSet == null || queryResult.DataSet.Tables.Count == 0)
                {

                    var stringBuilder = new StringBuilder();
                    int arg_53_0 = 0;
                    int num = queryResult.ValidationMessages.Count - 1;
                    for (int i = arg_53_0; i <= num; i++)
                    {
                        stringBuilder.Append(queryResult.ValidationMessages[i]);
                        stringBuilder.Append(Environment.NewLine);
                    }
                    var error = stringBuilder.ToString();

                    return new List<FoundationModel>();
                }
                //         0       1          2          3              4          5              6            7        8          9      10    11           12       13 
                /* 'Select ORG_ID, ORG_NAME, MEMSTATUS, ORG_COF_STATUS, ORG_CLASS, CONGRESSIONAL, ADDRESS_1, ADDRESS_2, ADDRESS_3, CITY, STATE, POSTAL_CODE, COUNTRY, ACCREDITED '     */

                var foundations = new List<FoundationModel>();

                if (queryResult.DataSet.Tables[0].Rows.Count > 0)
                {
                    var dt = queryResult.DataSet.Tables[0];

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        var s = dt.Rows[i][0].ToString();
                        var id = 0;
                        Int32.TryParse(s, out id);

                        foundations.Add(new FoundationModel
                        {
                            Id = id,
                            OrgName = dt.Rows[i][1].ToString(),
                            District = dt.Rows[i][5].ToString(),
                            CFNSB = dt.Rows[i][3].ToString(),
                            CouncilStatus = dt.Rows[i][2].ToString(),
                            Address1 = dt.Rows[i][6].ToString(),
                            Address2 = dt.Rows[i][7].ToString(),
                            City = dt.Rows[i][9].ToString(),
                            State = dt.Rows[i][10].ToString()
                        });
                    }
                }

                return foundations;
            }
            catch (Exception ex)
            {
                return new List<FoundationModel>();
            }
        }

        public List<KeyValuePair<string, string>> GetAllStates()
        {
            try
            {
                PersonifyInitializer.Initialize(".\\", new ImmediateSeatInformationProvider());
                IStoredProcedureRequest storedProcedureRequest = new StoredProcedureRequest("USR_COF_CONGRESSIONAL_DISTRICT_STATES_SP");

                IQueryRequest request = new QueryRequest(storedProcedureRequest);
                IQueryResult queryResult = Global.App.Execute(request);

                var states = new List<KeyValuePair<string, string>>();

                if (!queryResult.Success || queryResult.DataSet == null || queryResult.DataSet.Tables.Count == 0)
                {

                    var stringBuilder = new StringBuilder();
                    int arg_53_0 = 0;
                    int num = queryResult.ValidationMessages.Count - 1;
                    for (int i = arg_53_0; i <= num; i++)
                    {
                        stringBuilder.Append(queryResult.ValidationMessages[i]);
                        stringBuilder.Append(Environment.NewLine);
                    }
                    var error = stringBuilder.ToString();

                    return states;
                }


                if (queryResult.DataSet.Tables[0].Rows.Count > 0)
                {
                    var dt = queryResult.DataSet.Tables[0];

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        states.Add(new KeyValuePair<string, string>(dt.Rows[i][0].ToString(), dt.Rows[i][0].ToString()));
                    }
                }

                return states;
            }
            catch (Exception ex)
            {
                return new List<KeyValuePair<string, string>>();
            }

        }

    }
}