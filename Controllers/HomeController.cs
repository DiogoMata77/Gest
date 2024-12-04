using Gest.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Drawing;
using System.Globalization;
using System.Security.Cryptography.Xml;
using System.Web.Helpers;

namespace Gest.Controllers
{
    public class HomeController : Controller
    {
        public string con = "";
        public string query = "";
        SqlConnection sqlcon;
        SqlCommand sqlcom;
        SqlDataReader sdr;
        private readonly IConfiguration _configuration; 
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
            con = _configuration["Data:Conection"];
        }

        //------------------------------------Log In ---------------------------------------------------------------------------------------------------------------
        [HttpGet]
        [Route("/ConfigCookies")]
        //Configurar as cookies de sessao
        public void ConfigCookies()
        {
            try
            {
                var cookieOptions = new CookieOptions
                {
                    Expires = DateTimeOffset.Now.AddMinutes(45),
                    HttpOnly = true, // Optional: makes the cookie accessible only by the server
                    Secure = true, // Optional: ensures the cookie is only sent over HTTPS
                    SameSite = SameSiteMode.Strict // Optional: controls when the cookie is sent
                };

                // Set the cookie
                Response.Cookies.Append("Sessao", "YourSessionValue", cookieOptions);
            }
            catch (Exception ex)
            {

            }

        }

        [HttpPost]
        [Route("/ValidateLogIn")]
        public IActionResult ValidateLogIn(string nome, string senha)
        {
            if (ValidateInputs(nome, senha))
            {
                if (ValidateLogInData(nome, senha))
                { 
                    ConfigCookies();
                    return Json(new { success = true, url = "/Home/Index" });
                }
                else
                {
                    return Json(new { success = false, url = "Esta conta não foi encontrada por favor tente novamente!" });
                }
            }
            else
            {
                return Json(new { success = false, url = "Os campos não estão preenchidos corretamente!" });
            }
        }

        public bool ValidateInputs(string nome, string senha)
        {
            if (!string.IsNullOrEmpty(nome) && !string.IsNullOrEmpty(senha))
            {
                return true;
            }
            return false;
        }

        public bool ValidateLogInData(string nome, string senha)
        {
            int numDeUtilizadores = 0;
            using (sqlcon = new SqlConnection(con))
            {
                sqlcom = new SqlCommand("select count(*) as numutilizadores from Utilizadores where Email = @Email and Password = @Password", sqlcon);
                sqlcom.Parameters.AddWithValue("@Email", nome);
                sqlcom.Parameters.AddWithValue("@Password", senha);
                sqlcon.Open();
                //Retorna o primeiro valor da tabela
                numDeUtilizadores = (int)sqlcom.ExecuteScalar();
            }
            if(numDeUtilizadores == 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        public IActionResult LogIn()
        {
            //Valida se o valor da cookie é vazio ou nao
            if(!Request.Cookies.TryGetValue("Sessao", out string sessionValue))
            {
                ViewData["Title"] = "LogIn";
                return View();
            }
            else
            {
                return RedirectToAction("Index");
            }
            
        }

        [HttpGet]
        [Route("/LogOut")]
        public string LogOut()
        {
            //Para apagar as cookies
            Response.Cookies.Delete("Sessao");
            return "/";
        }

        //------------------------------------ Create Acount ---------------------------------------------------------------------------------------------------------------


        public IActionResult CreateAcount()
        {
            ViewData["Title"] = "Create Acount";
            return View();
        }

        [HttpPost]
        [Route("/FunctionCreate")]
        public IActionResult FunctionCreate(string nome, string email,string senha, string senhaConfirm)
        {
            if(ValidarCampos(nome, email, senha, senhaConfirm) == "p")
            {
                if(ValidarPasswords(senha, senhaConfirm) == "p")
                {
                    if(Create(nome, email, senha) == "p")
                    {
                        return Json(new { success = true, url = "/Home/LogIn" });
                    }
                    else
                    {
                        return Json(new { success = false, url = Create(nome, email, senha) });
                    }
                }
                else
                {
                    return Json(new { success = false, url = "As passwords tem valores diferentes" });
                }
            }
            else
            {
                return Json(new { success = false, url = "Preencha os campos corretamnete"});
            }
            
        }

        //Valida se os campos estao vazios
        public string ValidarCampos(string nome, string email, string senha, string senhaConfirm)
        {
            if (!string.IsNullOrEmpty(nome) && !string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(senha) && !string.IsNullOrEmpty(senhaConfirm))
            {
                return "p";
            }
            return "";
           
            
        }

        //Valida se as passwords sao iguais
        public string ValidarPasswords(string senha, string senhaConfirm)
        {
            if (senha.Trim() == senhaConfirm.Trim())
            {
                return "p";
            }
            return "";
        }

        //Valida se a informacao ja existe na base de dados para depois criar a conta
        public string Create(string nome, string email, string senha)
        {
            int numDeUtilizadores = 0;
            using (sqlcon = new SqlConnection(con))
            {
                sqlcom = new SqlCommand("select count(*) as numutilizadores from Utilizadores where Email = @Email", sqlcon);
                sqlcom.Parameters.AddWithValue("@Email", nome);
                sqlcon.Open();
                //Retorna o primeiro valor da tabela
                numDeUtilizadores = (int)sqlcom.ExecuteScalar();
                sqlcon.Close();
            }
            
            if (numDeUtilizadores == 0)
            {
                using (sqlcon = new SqlConnection(con))
                {
                    sqlcom = new SqlCommand("select count(*) as numutilizadores from Utilizadores where Nome = @Nome", sqlcon);
                    sqlcom.Parameters.AddWithValue("@Nome", nome);
                    sqlcon.Open();
                    //Retorna o primeiro valor da tabela
                    numDeUtilizadores = (int)sqlcom.ExecuteScalar();
                    sqlcon.Close();
                }
                if(numDeUtilizadores == 0)
                {
                    using (sqlcon = new SqlConnection(con))
                    {
                        sqlcom = new SqlCommand("insert into Utilizadores (Nome,Email,Password) values (@Nome,@Email,@Password)", sqlcon);
                        sqlcom.Parameters.AddWithValue("@Nome", nome);
                        sqlcom.Parameters.AddWithValue("@Email", email);
                        sqlcom.Parameters.AddWithValue("@Password", senha);
                        sqlcon.Open();
                        sqlcom.ExecuteNonQuery();
                        sqlcon.Close();
                    }
                        return "p";
                }
                else
                {
                    return "Este nome de utilizador já esta a ser utilizado!";
                }
            }
            else
            {
                return "Este email já esta a ser utilizado!";
            }
            
        }

        // ----------------------------------- Index -----------------------------------------------------------------------------------------------------------------


        [HttpPost]
        [Route("/GenerateProducts")]
        public string GenerateProducts(int state,string pesquisa)
        {
            if(string.IsNullOrEmpty(pesquisa))
            {
                query = "select * from Product where Mine = " + state + " and State = 1 ";
            }
            else
            {
                query = "select * from Product where Mine = " + state + " and Name like '%"+ pesquisa +"%' and State = 1 ";
            }
            
            sqlcon = new SqlConnection(con);
            sqlcom = new SqlCommand(query, sqlcon);
            sqlcon.Open();
            sdr = sqlcom.ExecuteReader();
            while (sdr.Read())
            {
                ViewBag.Content += "<tr>";
                ViewBag.Content += "<td><input onchange='selectProduct(this)' id='" + sdr["Id"].ToString() + "' class='centContent' type='checkbox'/></td>";
                ViewBag.Content += "<th>" + sdr["Name"].ToString() + "</th>";
                if (String.IsNullOrEmpty(sdr["Image"].ToString()))
                {
                    ViewBag.Content += "<td><p>Sem foto<p/></td>";
                }
                else
                {
                    ViewBag.Content += "<td><img class='productImg' alt='Product Image' src='" + sdr["Image"].ToString() + "'/></td>";
                }
                ViewBag.Content += "<td><a class='link' href='" + sdr["ProductLink"].ToString() + "' target=\"_blank\">" + sdr["ProductLink"].ToString() + "</a></td>";
                ViewBag.Content += "<td>" + sdr["Quantity"].ToString() + "</td>";
                ViewBag.Content += "<td>" + sdr["BuingPrice"].ToString() + "€</td>";
                ViewBag.Content += "<td>" + sdr["SellingPrice"].ToString() + "€</td>";
                ViewBag.Content += "<td><img id='" + sdr["Id"].ToString() + "' onclick='confirm(this)' alt='edit' src='/assets/verifica.png' class='actionIcon'/></td>";//Vendido
                ViewBag.Content += "<td><img id='" + sdr["Id"].ToString() + "' onclick='edit(this)' alt='edit' src='/assets/edit.png' class='actionIcon'/></td>";//Edit
                ViewBag.Content += "<td><img id='" + sdr["Id"].ToString() + "' onclick='apagar(this)' alt='delete' src='/assets/delete.png' class='actionIcon'/></td>";//Delete
                ViewBag.Content += "</tr>";
            }
            sqlcon.Close();
            return ViewBag.Content;
        }

        //Outra opcao para gerar os produtos
        //public List<Produtos> GenerateProducts(int state)
        //{
        //    //Continuar a funcionalidade do entity framework para pegar a informação atravez dos models
        //    List<Produtos> produtos = new List<Produtos>();
        //    //string code = "";
        //    query = "select * from Product where Mine = " + state + " and State = 1";
        //    sqlcon = new SqlConnection(con);
        //    sqlcom = new SqlCommand(query, sqlcon);
        //    sqlcon.Open();
        //    sdr = sqlcom.ExecuteReader();
        //    while (sdr.Read())
        //    {
        //        Produtos produto = new Produtos
        //        {
        //            Id = Convert.ToInt32(sdr["Id"].ToString()),
        //            Name = sdr["Name"].ToString(),
        //            Image = sdr["Image"].ToString(),
        //            BuingPrice = Convert.ToDecimal(sdr["BuingPrice"].ToString()),
        //            SellingPrice = Convert.ToDecimal(sdr["SellingPrice"].ToString()),
        //        };
        //        produtos.Add(produto);
        //    }
        //    sqlcon.Close();
        //    return produtos;
        //}

        [HttpPost]
        [Route("/Add")]
        public string Add(string name,int mine,string productLink,int quantity, string Img,decimal buingPrice,decimal sellingPrice)
        {
            try
            {
                //Problema com o selling and buing price
                string mineDes = "";

                if (mine == 0)
                {
                    mineDes = "Compra";
                }
                else if (mine == 1)
                {
                    mineDes = "Venda";
                }
                if (Img == null || Img == "")
                {
                    Img = "/assets/logo.jpeg";
                }

                query = "insert into Product ([Name],Mine,MineDes,ProductLink,Image,State,BuingPrice,SellingPrice,Quantity,DataDeCompra) values ('" + name + "'," + mine + ",'" + mineDes + "','" + productLink + "','" + Img + "',1," + buingPrice + "," + sellingPrice + "," + quantity + ",'"+DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")+"') ";
                sqlcon = new SqlConnection(con);
                sqlcom = new SqlCommand(query, sqlcon);
                sqlcon.Open();
                sqlcom.ExecuteNonQuery();
                sqlcon.Close();

                if(mine != null)
                {
                    return GenerateProducts(mine,"");
                }          
                else
                {
                    return "error";
                }
            }
            catch (Exception ex) { return "error"; }
        }

        [HttpPost]
        [Route("/getProductValues")]
        //Buscar dados atraves do json
        public ActionResult getProductValues(int id)
        {
            string nome = "";
            string img = "";
            int destino = 0;
            string link = "";
            int quantity = 0;
            decimal venda = 0;
            decimal compra = 0;

            query = "select * from [Product] where Id = "+id;
            sqlcon = new SqlConnection(con);
            sqlcom = new SqlCommand(query, sqlcon); 
            sqlcon.Open();
            sdr = sqlcom.ExecuteReader();
            while(sdr.Read())
            {
                nome = sdr["Name"].ToString();
                img = sdr["Image"].ToString();
                destino = Convert.ToInt32(sdr["Mine"].ToString());
                link = sdr["ProductLink"].ToString();
                quantity = Convert.ToInt32(sdr["Quantity"].ToString());
                venda = Convert.ToDecimal(sdr["SellingPrice"].ToString());
                compra = Convert.ToDecimal(sdr["BuingPrice"].ToString());
            }
            sqlcon.Close();

            //Passar os dados para json para passar multiplos valores 
            var data = new {Destino = destino ,Nome = nome,Imagem =  img,Link = link,Quantidade =  quantity, Venda = venda, Compra = compra };
            var jsonData = JsonConvert.SerializeObject(data);
            return Content(jsonData, "application/json");
        }

        [HttpPost]
        [Route("/UpdateData")]
        public string UpdateData(int id, string name, int mine, string productLink, int quantity, string Img, decimal buingPrice, decimal sellingPrice, int function)
        {
            string mineDes = "";
            if (mine == 0)
            {
                mineDes = "Compra";
            }
            else if (mine == 1)
            {
                mineDes = "Venda";
            }
            if (!string.IsNullOrEmpty(Img))
            {
                query = "update Product set Name = '" + name + "',Mine = " + mine + ", MineDes = '" + mineDes + "', ProductLink = '" + productLink + "', [Image] = '" + Img + "', BuingPrice = " + buingPrice + ", SellingPrice = " + sellingPrice + ", Quantity = " + quantity + " where Id = " + id + "";
            }
            else
            {
                query = "update Product set Name = '" + name + "',Mine = " + mine + ", MineDes = '" + mineDes + "', ProductLink = '" + productLink + "', BuingPrice = " + buingPrice + ", SellingPrice = " + sellingPrice + ", Quantity = " + quantity + " where Id = " + id + "";

            }
            sqlcon = new SqlConnection(con);
            sqlcom = new SqlCommand(query, sqlcon);
            sqlcon.Open();
            sqlcom.ExecuteNonQuery();
            sqlcon.Close();

            if (function == 0)
            {
                return GenerateProducts(0,"");
            }
            else if (function == 1)
            {
                return GenerateProducts(1, "");
            }
            else
            {
                return "error";
            }
        }

        [HttpPost]
        [Route("/GetValues")]
        public JsonResult GetValues(string ids)
        {
            try
            {
                decimal compra = 0;
                decimal compraf = 0;
                decimal venda = 0;
                decimal vendaf = 0;
                int quantidade = 0;
                decimal total = 0;

                if (ids != null && ids.Contains(","))
                {
                    string[] id = ids.Split(",");
                    int virgulas = ids.Split(",").Length;

                    for (int i = 1; i < virgulas; i++)
                    {
                        query = "select BuingPrice,SellingPrice,Quantity from [Product] where Id =" + id[i];
                        sqlcon = new SqlConnection(con);
                        sqlcom = new SqlCommand(query, sqlcon);
                        sqlcon.Open();
                        sdr = sqlcom.ExecuteReader();
                        while (sdr.Read())
                        {
                            compra = Convert.ToDecimal(sdr["BuingPrice"].ToString());
                            compraf += Convert.ToDecimal(sdr["BuingPrice"].ToString());
                            venda = Convert.ToDecimal(sdr["SellingPrice"].ToString());
                            vendaf += Convert.ToDecimal(sdr["SellingPrice"].ToString());
                            quantidade = Convert.ToInt32(sdr["Quantity"].ToString());
                        }
                        sqlcon.Close();
                        total += (venda - compra) * quantidade;
                    }
                    
                }
                decimal[] myArray = { vendaf, compraf, total };
                return Json(myArray);

            }
            catch(Exception ex)
            {
                return Json(ex.Message);
            }
        }

        [HttpPost]
        [Route("/Delete")]
        public string Delete(string ids,int funcao)
        {
            try
            {
                string[] id = ids.Split(",");
                int virgulas = ids.Split(",").Length;

                for (int i = 1; i <= virgulas - 1; i++)
                {
                    query = "delete from [Product] where Id = " + id[i];
                    sqlcon = new SqlConnection(con);
                    sqlcom = new SqlCommand(query, sqlcon);
                    sqlcon.Open();
                    sqlcom.ExecuteNonQuery();
                    sqlcon.Close();
                }

                if(funcao ==  0)
                    return GenerateProducts(0, "");
                else if(funcao == 1)
                    return GenerateProducts(1, "");
                else
                    return "error";
                 
            }
            catch(Exception ex) 
            {
                return ex.Message;
            }
        }

        [HttpPost]
        [Route("/UpdateMine")]
        public string UpdateMine(int id)
        {
            query = "Update dbo.Product set Mine = 1 and MineDes = 'Venda' and DataDeVenda = '" + DateTime.Now.ToString("dd//MM/yyyy HH:mm:ss") + "' where Id = "+id+"";
            sqlcon = new SqlConnection(con);
            sqlcom = new SqlCommand(query,sqlcon);
            sqlcon.Open();
            sqlcom.ExecuteNonQuery();
            sqlcon.Close();

            return GenerateProducts(0, "");
        }

        public IActionResult Index()
        {
            try
            {
                //Para pegar o valor das cookies
                var cookieValue = Request.Cookies["Sessao"];
                if (cookieValue != null)
                {
                    ViewData["Title"] = "Home";
                    GenerateProducts(0, "");
                    return View();
                }
                else
                {
                    return View("LogIn");
                }

            }
            catch(Exception ex)
            {
                return Error();
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        // ------------------------------------------------------------------------------------------------- Despezas Code ------------------------------------------------------------------------------------------------

        public IActionResult Despezas()
        {
            List<DespezasClass> myDespezas = GetDespezas();

            return View(myDespezas);
        }

        public List<DespezasClass> GetDespezas()
        {
            List<DespezasClass> myDespezas = new List<DespezasClass>();
            using (sqlcon = new SqlConnection(con))
            {
                sqlcon.Open();
                sqlcom = new SqlCommand("Select * from despezas order by Data desc", sqlcon);
                SqlDataReader sdr = sqlcom.ExecuteReader();
                while(sdr.Read())
                {
                    DespezasClass despeza = new DespezasClass
                    {
                        Id = Convert.ToInt32(sdr["id"]),
                        Desc = sdr["Desc"].ToString(),
                        Price = Convert.ToDecimal(sdr["Valor"].ToString()),
                        Finalidade = Convert.ToInt32(sdr["Finalidade"]),
                        Data = Convert.ToDateTime(sdr["Data"].ToString()),
                    };
                    myDespezas.Add(despeza);
                }
                sqlcon.Close();
            }

            return myDespezas;
        }

        //public void GetDespezas()
        //{
        //    List<DespezasClass> myDespezas = new List<DespezasClass>();
        //    using (sqlcon = new SqlConnection(con))
        //    {
        //        sqlcon.Open();
        //        sqlcom = new SqlCommand("Select top (7) * from despezas where Finalidade = 0 order by Data desc", sqlcon);
        //        SqlDataReader sdr = sqlcom.ExecuteReader();
        //        while (sdr.Read())
        //        {
        //            if (sdr["Finalidade"].ToString() == "0")
        //            {
        //                ViewBag.Despeza += sdr["Desc"].ToString() + ";";
        //                ViewBag.Preco += sdr["Valor"].ToString() + " €;";
        //                ViewBag.Id += sdr["Id"].ToString() + ";";
        //            }
        //            else if (sdr["Finalidade"].ToString() == "1")
        //            {

        //            }
        //        }
        //        sqlcon.Close();
        //    }
        //}
        //public void GetLucros()
        //{
        //    using (sqlcon = new SqlConnection(con))
        //    {
        //        sqlcon.Open();
        //        sqlcom = new SqlCommand("Select top (7) * from despezas  where Finalidade = 1 order by Data desc", sqlcon);
        //        SqlDataReader sdr = sqlcom.ExecuteReader();
        //        while (sdr.Read())
        //        {
        //            if (sdr["Finalidade"].ToString() == "1")
        //            {
        //                ViewBag.Lucros += sdr["Valor"].ToString() + " €;" + sdr["Desc"].ToString() + ";";
        //                ViewBag.PrecoL += sdr["Valor"].ToString() + " €;";
        //                ViewBag.IdL += sdr["Id"].ToString() + ";";
        //            }
        //        }
        //        sqlcon.Close();
        //    }
        //}

        [HttpPost]
        [Route("/InsertDespeza")]
        public void InsertDespeza(int valor,decimal montante,string descricao)
        {
            using ( sqlcon = new SqlConnection(con))
            {
                sqlcon.Open();
                sqlcom = new SqlCommand("INSERT INTO [dbo].[Despezas] ([Desc],[Valor],[Finalidade],[Finalidade_desc],[Data]) values ('" + descricao+"',"+montante+","+valor+ ",@finalidadeDesc,@data) ", sqlcon);
                if(valor == 0)
                {
                    sqlcom.Parameters.AddWithValue("@finalidadeDesc", "Despeza");
                }
                else if (valor == 1)
                {
                    sqlcom.Parameters.AddWithValue("@finalidadeDesc", "Lucros");
                }
                sqlcom.Parameters.AddWithValue("@data",DateTime.Now);
                sqlcom.ExecuteNonQuery();
                sqlcon.Close();
            }
            GetDespezas();
        }

        [HttpPost]
        [Route("/DeleteDespeza")]
        public void DeleteDespeza(int id)
        {
            using( sqlcon = new SqlConnection(con))
            {
                sqlcon.Open();
                sqlcom = new SqlCommand("delete from despezas where Id = " + id,sqlcon);
                sqlcom.ExecuteNonQuery();
                sqlcon.Close();
            }
        }

        public IActionResult Error404()
        {
            return View();
        }

        //--------------------------------------------------- Estatisticas ----------------------------------------------------------------------------------------

        public IActionResult Estatisticas()
        {
            var cookieValue = Request.Cookies["Sessao"];
            if (cookieValue != null)
            {
                var estatisticas = GetEstatisticas(Convert.ToInt32(DateTime.Now.Year.ToString()));
                GetAnos();

                return View(estatisticas);
            }
            else
                return View("LogIn");
        }

        

        [HttpPost]
        [Route("/GetEstatisticas")]
        public List<Estatisticas> GetEstatisticas(int ano)
        {
            //Criar uma variavel para retornar os valors para a view
            List<Estatisticas> estatisticas = new List<Estatisticas>();
           
            //Criar uma variavel do tipo da class para atribuire os valores a lista
            Estatisticas e = new Estatisticas
            {
                valorCompra = GetEstatisticasdata("Select ISNULL(sum(BuingPrice),0) as precoCompra from Product where YEAR(CONVERT(DATETIME,DataDeCompra, 103)) = @ano and MONTH(CONVERT(DATETIME,DataDeCompra,103)) = @mes", ano,"precoCompra"),
                valorVenda = GetEstatisticasdata("Select ISNULL(sum(SellingPrice),0) as precoVenda from Product where YEAR(CONVERT(DATETIME,DataDeVenda, 103)) = @ano and MONTH(CONVERT(DATETIME,DataDeVenda,103)) = @mes", ano,"precoVenda"),
                Lucro = GetEstatisticasdata("select isnull(sum(SellingPrice - BuingPrice),0) as lucro from Product where Mine = 1 and YEAR(CONVERT(DATETIME,DataDeVenda, 103)) = @ano and MONTH(CONVERT(DATETIME,DataDeVenda,103)) = @mes", ano,"lucro")
            };
            estatisticas.Add(e);

            return estatisticas;
        }

        public List<decimal> GetEstatisticasdata(string query,int ano,string key)
        {
            List<decimal> lucro = new List<decimal>();
            for (int i = 1; i <= 12; i++)
            {
                using (sqlcon = new SqlConnection(con))
                {
                    sqlcon.Open();
                    sqlcom = new SqlCommand(query, sqlcon);
                    sqlcom.Parameters.AddWithValue("@ano", ano);
                    sqlcom.Parameters.AddWithValue("@mes", i);
                    sdr = sqlcom.ExecuteReader();
                    while (sdr.Read())
                    {
                        decimal valores = Convert.ToInt32(sdr[key].ToString());

                        lucro.Add(valores);
                    }
                    sqlcon.Close();
                }
            }

            return lucro;
        }

        public void GetAnos()
        {
            //Usamos uma list porque não sabemos o tamanho do array
            List<int> anos = new List<int>();

            //O distinct vai buscar apenas um valor caso eles sejam repetidos e o union junrta os dois select para pegar dos dois campos
            query = "select distinct YEAR(CONVERT(DATETIME,DataDeCompra, 103)) as ano from Product where DataDeCompra is not null Union select distinct YEAR(CONVERT(DATETIME,DataDeVenda, 103)) from Product  where DataDeVenda is not null order by ano ASC";
            using (sqlcon = new SqlConnection(con))
            {
                sqlcom = new SqlCommand(query, sqlcon);
                sqlcon.Open();
                sdr = sqlcom.ExecuteReader();
                while (sdr.Read())
                {
                    anos.Add(Convert.ToInt32(sdr["ano"].ToString()));
                }
            }

            //Para guardar em formato de array
            ViewData["Anos"] = anos.ToArray();
        }
    }
}