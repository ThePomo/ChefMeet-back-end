👨‍🍳🍝 ChefMeet – Il Marketplace degli Chef a Domicilio Benvenuto su ChefMeet, la piattaforma web che connette Chef professionisti e utenti appassionati di buona cucina. Gli chef possono creare profili, proporre eventi privati, pubblicare ricette, e impostare la propria disponibilità. Gli utenti possono esplorare, prenotare e interagire con i contenuti.

🚀 Tecnologie utilizzate ⚙️ Backend: ASP.NET Core Web API + Entity Framework Core + SQL Server

🎨 Frontend: React + Bootstrap

🔒 Autenticazione: ASP.NET Identity + JWT

📦 Gestione stato: Redux

🖼️ File upload: immagini profilo e immagini ricette/eventi

📅 Calendario eventi: gestione disponibilità personalizzate

👥 Tipi di Utenti Ruolo Funzionalità 👤 Utente Registrazione, prenotazione eventi, ricerca chef/utenti, like alle ricette 🍳 Chef Profilo pubblico, gestione disponibilità, creazione eventi, creazione ricette 🛡️ Admin Accesso alla dashboard amministrativa (WIP)

🛠️ Funzionalità principali ✅ Autenticazione 🔐 Registrazione con scelta del ruolo (Utente o Chef)

🔑 Login con JWT + Redux

👤 Modifica profilo utente/chef con upload immagine

👨‍🍳 Chef 📋 Profilo pubblico e modificabile

🧾 Bio, città e immagine profilo

📅 Gestione calendario disponibilità

🧑‍🍳 Creazione eventi privati con data, prezzo e descrizione

🍽️ Creazione e pubblicazione ricette con immagine

👨‍👩‍👧‍👦 Utente 🔍 Ricerca chef o utenti tramite barra in navbar

🗓️ Prenotazione eventi privati

❤️ Like alle ricette

📋 Visualizzazione prenotazioni effettuate

🏠 Home page dinamica 👋 Messaggio di benvenuto se non loggato

🔥 Eventi e Ricette in evidenza per utenti loggati

🧑‍🍳 Nome chef cliccabile → profilo pubblico

📸 Immagine ricetta + like integrato

🧪 API principali (REST) Metodo Endpoint Descrizione GET /api/Evento Elenco eventi POST /api/Evento Creazione evento (con immagine) GET /api/Chef/byUser/{userId} Dettagli chef dal profilo PUT /api/Chef/{id} Modifica profilo chef (con immagine) GET /api/Creazione Ricette pubbliche POST /api/Creazione Creazione ricetta (con immagine) PUT /api/Creazione/{id} Modifica ricetta (facoltativa immagine)

🖼️ Upload Immagini ✅ Supportato per: profilo utente, profilo chef, eventi, ricette

🔎 Ricerca utenti & chef

🔍 Live Search integrata nella navbar con risultati dropdown cliccabili:

👤 Utente → /utente/{id}

🍳 Chef → /chef/{chefId}

🔄 In sviluppo (WIP)

Sistema di recenzioni 

Sistema chat privata tra utenti

sistema commenti
📬 Email di conferma prenotazione

💳 Integrazione pagamento eventi

🗳️ Sistema recensioni chef e valutazioni

🧑‍💻 Autore Realizzato con ❤️ come progetto full stack completo, per mostrare le competenze in React, .NET, SQL, autenticazione JWT e UI moderna.

Link al Front-End https://github.com/ThePomo/chefmeet-frontend
Made By Giuseppe Pomo
