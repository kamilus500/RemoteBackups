🛡️ RemoteBackups

Aplikacja kliencko-serwerowa do bezpiecznego przechowywania i zarządzania kopiami zapasowymi. Projekt został zbudowany z naciskiem na skalowalność, czystość kodu oraz najwyższą wydajność, stanowiąc praktyczną implementację nowoczesnych wzorców projektowych w ekosystemie .NET.

💡 Koncepcja Architektoniczna

Projekt zostały zaimplementowany używając Vertical Slice Architecture. Kod REST API jest podzielony ze względu na funkcjonalności biznesowe (Features).

Dzięki temu każdy "pionowy plaster" (np. Upload) jest w pełni autonomicznym bytem, od obsługi żądania HTTP po operacje na danych, co idealnie współgra z podejściem CQRS. Minimalizuje to sprzężenia, ułatwia testowanie i sprawia, że aplikacja jest gotowa na rozwój w realiach enterprise.

🚀 Stos Technologiczny

Backend (REST API)

.NET 10

Vertical Slices & Minimal APIs – maksymalna spójność kodu wokół funkcji biznesowych.

Entity Framework Core

JWT (JSON Web Tokens) – bezstanowa, bezpieczna autoryzacja i autentykacja.

Frontend (SPA)

Blazor WebAssembly – interaktywny interfejs renderowany po stronie klienta, napisany w całości w C#.

✨ Główne Funkcjonalności

Pełne zarządzanie cyklem życia plików – płynny upload, bezpieczne pobieranie i usuwanie kopii zapasowych.

Moduł tożsamości – kompletny proces rejestracji i logowania z zabezpieczeniem dostępu do zasobów.

Dashboard użytkownika – przejrzysty widok metadanych (rozmiary plików, daty utworzenia).

🧪 Testowanie i Zapewnienie Jakości

Testy jednostkowe: Weryfikują kluczową logikę biznesową wewnątrz izolowanych handlerów, bez angażowania infrastruktury. Służą do błyskawicznego sprawdzania reguł walidacji i transformacji danych.

Testy integracyjne z Testcontainers: Zamiast używać wbudowanych atrap (mocków) czy baz in-memory, aplikacja wykorzystuje bibliotekę Testcontainers. Automatycznie podnosi ona rzeczywiste kontenery Dockerowe (np. relacyjną bazę danych) na czas trwania testów. Daje to stuprocentową pewność, że cała ścieżka od requestu HTTP, przez bazę danych, aż po zwrócenie odpowiedzi działa poprawnie.

Aby uruchomić wszystkie testy (wymagany uruchomiony Docker Desktop), użyj komendy:
dotnet test
