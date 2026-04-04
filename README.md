\# eGK-Reader-API



This Software should have the capability to read the German Health card (a type of an smartcard.

The specification should be this one \[https://fachportal.gematik.de/fileadmin/user\_upload/fachportal/files/Spezifikationen/Basis-Rollout/Elektronische\_Gesundheitskarte/gemLF\_Impl\_eGK\_V160.pdf](https://fachportal.gematik.de/fileadmin/user\_upload/fachportal/files/Spezifikationen/Basis-Rollout/Elektronische\_Gesundheitskarte/gemLF\_Impl\_eGK\_V160.pdf)



The Application should have one endpoint to request the data from the currently present card. The client should be able to specify the reader to use or a default one should be choosen. If no card is present no data should be returned.

Clients should identify themselfs properly before they can access the data. (JWT or mTLS should be viable)

Cors should be configurable.



