# Projekto aprašymas

Kuriamas projektas – 2D laivų mūšio žaidimas. Žaidimas yra skirtas žaisti poroje su priešininku tinkle. Kaip ir originaliame žaidime kiekvienas žaidėjas turės lentą kurioje sudės savo laivus ir kitą lentą, kur bandys nuskandinti priešininko laivus. Žaidimą sudarys skirtingų formų laivai ir skirtingi ginklai kariauti prieš priešininką. Progresuodamas žaidėjas gali atsirakinti naujus ginklus ir žemėlapius (skirtingus žemėlapio dydžius). Žaidėjas pagrinde galės valdyti viską su pele arba klavišais norint pasukti laivą. Šio žaidimo tikslas yra sukurti žaidimą panašų į “BattleShip” su daugiau įvairovės (pav. Žr. 1 pav.).

Komandos nariai:
- Marius Kamarauskas
- Laurynas Stripeika
- Aurimas Kliokys
- Ričardas Bielskas


## Žaidimo lygiai

### Lygis 1
- Įprastas lygis. Žaidėjas pasirenka, kur nori dėti laivelius ekrane. Abiem žaidėjams patvirtinus, pradedamas žaidimas. Žaidėjai gauna paprastas bombas, paspaudus ant norimo priešininko kvadratėlio ir pataikius, žaidėjo laivas praranda gyvybes ir parodomas priešininko laivas. Taip pat kiekvieno ėjimo metu yra ribotas laikas.
- Kiekviena skirtinga bomba muša skirtinga kiekį langelių.
- Žaidėjas pasirenka savo atakavimo laivą. Atakos metu damage skaičiavimas parenkamas pagal 4 parametrus: Temą, Atakuojančio laivo tipą, Gyvų laivų kiekį, bei atakos tipą.

### Lygis 2
- Prasideda kai atstatomas žaidimas „reset“. Atstatomi visi žaidėjo laivai, tačiau jis turi tą patį kiekį taškų kiek turėjo prieš atstatymą. Laimi tas, kuris numuša visus priešininko taškus arba kuriam baigiasi ėjimai.

## Žaidimo tikslas
Žaidimo laimėtojas bus žaidėjas numušęs visus priešininko laivus, bei strateguoti kadangi ėjimų skaičius ribotas.

### Naudojamos technologijos
- Komunikacijai tarp žaidėjų: SignalR.
- Programavimo kalbos: C#.
- Grafinis atvaizdavimas: WPF Forms biblioteka
- Serveris: Debian Linux, Docker

# Projekto reikalavimai

## Funkciniai reikalavimai

1. Prisijungimas:
   - Žaidėjas gali prisijungti prie žaidimo sistemos.
   - Laukiama, kol prisijungs bent du žaidėjai prie žaidimo.

2. Pranešimai:
   - Žaidėjai gauna pranešimus apie įvykius žaidime, pvz., kada kitas žaidėjas prisijungia arba kada jie gali pasirinkti laivų dedamąją vietą.
   - Pataikymas atakos į priešininko laivą.
   - Nepataikymas atakos į priešininko laivą.
   - Laivų taškai.

3. Laivų dedamoji vieta:
   - Žaidėjai gali susidėlioti savo laivus ant žemėlapio.
   - Galimybė pasirinkti atsitiktinį laivų išdėstymą.

4. Papildomų laivų padėjimas:
   - Yra maksimalus papildomų laivų padėjimo skaičius.
   - Žaidėjai gali pasirinkti papildomų laivų padėjimą, jei jų skaičius yra neviršijamas.

5. Pasiruošimas žaidimui:
   - Žaidėjai gali paspausti mygtuką "ready", kai jie yra pasiruošę pradėti žaidimą.
   - Žaidimas prasideda tik tada, kai abu žaidėjai yra pasiruošę.

6. Žaidimo eiga:
   - Pradeda pirmasis prisijungęs žaidėjas.
   - Žaidėjai atlieka ėjimus pakaitomis.
   - Kiekvienas ėjimas apima laivo šūvį į priešininko laivą.
   - Žaidėjas gali pasikeisti tematiką, tai lemia atakos damage skaičiavimą(diena/naktis)
   - Žaidėjas pasirenka atakavimo laivą ir bombos tipą, paskaičiuojamos koordinates pagal bombos tipą bei laivo dydį.
   - Derinimo režimas: matomi priešininko laivai.
   - Žaidėjas gali pasirinkti ar laivelis pakelia vėliavėlę ir kokios jis bus spalvos.
   - Žaidėjas gali per tam tikrą laiką atlikti šūvį pakartotinai, jeigu jis nepataikė į priešininko laivą.
   - Žaidėjas gali perkrauti žaidimą iš naujo.
   - Žaidėjas turi 10 ėjimų per kuriuos turi numušti visus priešininko laivus, laimi žaidėjas kuriam greičiausiai baigiasi ėjimai.
   - Kiekvienas laivas turi ribotą šūvių kiekį, jeigu laivas nebeturi šūvių, jisai dingsta iš sąrašo, o jei visi laivai nebeturi šūvių baigiamas žaidimas.
   - Atakos damage skaičiuojamas taip:
     - Temos tipas
       - Diena + 300
       - Naktis + Random nuo 0 iki 100
     - Laivo tipas
       - Big Ship + 300
       - Medium ship + 100
       - Small ship + 10
     - Fleet damage
       - 2 > laivai damage padauginamas iš 2
       - 3 > laivai damage padauginamas iš 3
     - Atakos tipas
       - BigShipAttack + 100
       - MediumShipAttack + 200
       - SmallShipAttack + 50
   - Pataikius į visas laivo koordinates, jo gyvybes tampa 0

## Nefunkciniai reikalavimai

7. Sąsajos naudojimas:
   - Vartotojo sąsaja turi būti aiški ir lengvai suprantama, leidžiant žaidėjams patogiai sąveikauti su sistema.
   - Vartotojas gali pažymėti savo laivus priešininkui, keisti jų spalvą.

8. Lengvumas:
   - Žaidimo taisyklės ir sąsajos dizainas turi būti lengvai suprantamas naujiems žaidėjams.

9. Apribojimai:
   - Riboti laivų padėjimą pagal nustatytą maksimalų skaičių, kad išvengtume žaidimo disbalanso.
  
## Docker paleidimas
- docker build -t ships .
- docker run -p 8080:80 -t ships 
Pasikeisti IP WPF Windows
