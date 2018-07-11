using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESLTracker.DataModel;
using ESLTracker.Utils;

namespace ESLTracker.BusinessLogic.Cards
{
    public class CardsDatabase : ICardsDatabase
    {

        // pairs old, new guid
        internal static Dictionary<string, string> GuidTranslation = new Dictionary<string, string>()
        {
            { "af763bc0-b6bd-4d33-b229-36473bcf5a51", "925259a6-95df-4d97-806d-895e3f2605a4" }, //Aela the Huntress
            { "ecf450ad-af09-438c-90ea-a6d58e6b3d53", "17d93fdd-e7b8-46b5-8a61-a459ab8ff9bf" }, //Aela's Huntmate
            { "7843fde3-4596-45f3-acbc-8ffc376597d7", "406c7363-e7af-4afb-82fd-2361c6059db9" }, //Circle Initiate
            { "5b569f48-f88f-11e6-bc64-92361f002671", "40813eec-294a-4f53-b5ba-2b68bd286468" }, //Companion Harbinger
            { "b27dfe4a-3948-44df-b82a-8e8ebcfbbf07", "1816a1ac-912d-44dd-9a40-c11343393eb1" }, //Grim Shield-Brother
            { "e62d3cc5-6650-11e6-bdf4-0800200c9a66", "17bc012e-3dc3-4b69-b276-1531c82b4660" }, //Whiterun Protector
        };

        // pairs new guid, old guid
        //for time being new guid in file, and old in cards.json
        internal static Dictionary<string, string> GuidTranslationCore = new Dictionary<string, string>()
        {
            //core guids fix
            {"094f4937-e5db-8a41-e441-1905cd7a1406","7e686205-20ce-11e5-867f-0800200c9a66"}, //Abecean Navigator
            {"013fc94d-efb5-ae82-31c8-cd27348b730a","51f31711-300a-11e5-a2cb-0800200c9a66"}, //Adoring Fan
            {"90f7dc80-21ed-d0c6-dfca-10f82e990600","beadd0ce-4596-11e3-8f96-0800200c9a66"}, //Afflicted Alit
            {"575f4010-2584-3a83-9637-e276cd7baacc","beb041d8-4596-11e3-8f96-0800200c9a66"}, //Ageless Automaton
            {"ab155105-ab84-9b42-8a37-1c0f79b0a92a","3d4251c0-7983-11e3-981f-0800200c9a66"}, //Ahnassi
            {"bde9b95e-0498-dbce-decd-31e7ca00ea18","7e686208-20ce-11e5-867f-0800200c9a66"}, //Aldmeri Patriot
            {"89d9e34a-3814-b4ad-ff87-5df0ccd71438","e967a7f1-a458-11e5-a837-0800200c9a66"}, //Alik'r Bandit
            {"bc679e34-fbe8-1787-6aba-7170e40322d8","fbcc37ee-78b7-11e3-981f-0800200c9a66"}, //Alik'r Survivalist
            {"36597b39-3783-6b99-af86-b314063b5d8a","beb041cb-4596-11e3-8f96-0800200c9a66"}, //Allena Benoch
            {"723e4975-156b-148b-50a0-8027c1d7b560","0356f019-ff60-11e4-b939-0800200c9a66"}, //Alpha Wolf
            {"80155aee-eea2-c528-258c-d2739576141e","037320e8-a710-42df-a099-f864fadfe673"}, //Altar of Despair
            {"63fb346d-d3c7-069a-2ed9-fc4b900ebaf4","0dadeafb-0efb-11e5-b939-0800200c9a66"}, //Angry Grahl
            {"046cdabd-dee9-4157-693f-d2e100fdd0c0","51f31716-300a-11e5-a2cb-0800200c9a66"}, //An-Xileel Invader
            {"02caf274-7704-2298-e0db-dd35f87fbe7e","d0d4131d-a8a0-11e3-a5e2-0800200c9a66"}, //Apprentice's Potion
            {"7f808cb6-b5e3-1891-00cd-eb1bdfffcba5","5e26d8d5-f451-11e4-b939-0800200c9a66"}, //Archein Elite
            {"7c5d6525-45a6-3785-a334-d9eb33608774","0e06d342-1112-11e5-b939-0800200c9a66"}, //Archein Guerrilla
            {"75305abc-2d8b-7607-6f19-81e4946b0eae","086621f3-ff5d-11e4-b939-0800200c9a66"}, //Archein Venomtongue
            {"2dc4da86-b41f-039f-af2a-cffdab93adb1","beb041c1-4596-11e3-8f96-0800200c9a66"}, //Arenthia Swindler
            {"c7b2033d-ea84-40ec-d143-4a87b0b30c1b","aab8e4b0-f7fc-11e3-a3ac-0800200c9a66"}, //Arrow in the Knee
            {"72ab8683-9d5b-013a-ec0a-555251db566f","00f3994d-52f0-11e3-8f96-0800200c9a66"}, //Arrow Storm
            {"e422cd22-78f2-8eb6-b5af-6ed4af4dc9a3","beaff3be-4596-11e3-8f96-0800200c9a66"}, //Artaeum Savant
            {"33f56f09-756b-f822-203a-12b700ab760e","beadf7d1-4596-11e3-8f96-0800200c9a66"}, //Ash Servant
            {"496f60b2-6977-ba53-d125-90d5bcd5e15f","8e0d6f61-f400-11e4-b939-0800200c9a66"}, //Assassin's Bow
            {"49c26ce1-db15-b23e-885d-00ffb01b92e9","0e06fa5f-1112-11e5-b939-0800200c9a66"}, //Auridon Paladin
            {"359b887a-fb19-5d4c-30c5-59bef114792e","51f31717-300a-11e5-a2cb-0800200c9a66"}, //Auroran Sentry
            {"ee78dd77-6aa0-358b-4473-877856a1a88d","99c98991-b3a3-11e3-a5e2-0800200c9a66"}, //Ayrenn
            {"88d26721-93bb-427f-ae58-255d58bedb14","8e0d6f6c-f400-11e4-b939-0800200c9a66"}, //Baandari Bruiser
            {"6625895b-103d-3bf7-45dc-2ab0f20a139f","8e0d6f64-f400-11e4-b939-0800200c9a66"}, //Balmora Spymaster
            {"39ddb301-92a3-00ad-a820-9b3011a566d0","441521f1-300a-11e5-a2cb-0800200c9a66"}, //Bangkorai Butcher
            {"9af21a6d-50d2-7ea7-c3ea-d517e00739a3","086621fb-ff5d-11e4-b939-0800200c9a66"}, //Barded Guar
            {"5431d81e-8b2c-c863-a3b0-25c52cb18aaf","7e686206-20ce-11e5-867f-0800200c9a66"}, //Baron of Tear
            {"7257cc23-2842-1cfb-2a6d-ca85d257cfd8","beadd0c7-4596-11e3-8f96-0800200c9a66"}, //Battlerage Orc
            {"83513857-ba27-9004-d4bd-98fc679017cc","36eb2738-c160-11e5-a837-0800200c9a66"}, //Battlereeve of Dusk
            {"86a5c6a5-6132-a7ec-91be-abacb61dc1b5","9b19967b-f70b-4b96-981a-83fa46047d45"}, //Belligerent Giant
            {"14c71ca5-850b-140c-a8a8-770dc37c6b7f","fd235b50-0afb-11e5-b939-0800200c9a66"}, //Black Marsh Warden
            {"4902fe0e-1746-7460-6dbc-e4a011447ebc","beafccba-4596-11e3-8f96-0800200c9a66"}, //Black Worm Necromancer
            {"ed8e17c6-5ea6-6a69-befe-e486c9639ec5","6e153755-f451-11e4-b939-0800200c9a66"}, //Blackmail
            {"9bd81fd5-330d-7dca-4cd7-42a5e13958b6","fa526152-241a-11e4-8c21-0800200c9a66"}, //Blackrose Herbalist
            {"8c2e3b78-c7e7-b7a7-932d-c23ab8cb2d79","d0d3ebf0-a8a0-11e3-a5e2-0800200c9a66"}, //Blacksap Protector
            {"e04cd00f-8242-9ebb-292b-ea57855f49d7","27a2d0a9-3865-4f89-ad2e-766ab76d1b24"}, //Blighted Alit
            {"926bc686-ffca-9faa-431d-3787bf15f1ce","20b9deea-66b6-11e3-949a-0800200c9a66"}, //Blood Dragon
            {"d7aa55e5-8068-f234-0857-5f9a4d447d02","7e6861fb-20ce-11e5-867f-0800200c9a66"}, //Blood Magic Lord
            {"cffca262-39e5-a9bc-2782-fa081ba832b5","d0d41301-a8a0-11e3-a5e2-0800200c9a66"}, //Bog Lurcher
            {"1bf4577b-7360-2326-0b93-a3d359ce2007","0cd2564b-e227-11e3-8b68-0800200c9a66"}, //Bone Bow
            {"a8d2d0e0-0618-d17d-ddac-3263a45eac9b","fbcc5ef2-78b7-11e3-981f-0800200c9a66"}, //Bone Colossus
            {"b2352643-a016-0ddc-4af4-c603ebe79f5b","00f37258-52f0-11e3-8f96-0800200c9a66"}, //Breton Conjurer
            {"4a2815e6-7f17-540d-b20c-7e0bbf6b0d7a","fd235b59-0afb-11e5-b939-0800200c9a66"}, //Brilliant Experiment
            {"c6c6b50a-fa2a-0e85-5751-b4fcb3cde9e5","086621f4-ff5d-11e4-b939-0800200c9a66"}, //Bruma Armorer
            {"7b0bb4a6-d828-8762-940d-be5ec854b2fe","0cd2564e-e227-11e3-8b68-0800200c9a66"}, //Bruma Profiteer
            {"7a29f774-3915-7f43-9b22-f3fd7c980f8c","8e0d6f70-f400-11e4-b939-0800200c9a66"}, //Brutal Ashlander
            {"11d5b386-420f-9740-d542-4e1c17c1423d","33acb172-300a-11e5-a2cb-0800200c9a66"}, //Burn and Pillage
            {"1c9a97a5-99ab-159c-8118-e81aad88ff64","7e686204-20ce-11e5-867f-0800200c9a66"}, //Calm
            {"682d8887-6dfc-a3f7-91c4-c4c39c9d9329","7ec2bb50-b527-11e3-a5e2-0800200c9a66"}, //Camlorn Adventurer
            {"19c5d14e-c386-59e1-9bad-2e128a65665b","7ec2bb52-b527-11e3-a5e2-0800200c9a66"}, //Camlorn Hero
            {"bd9165ba-3129-fe9b-e868-79f861eccf04","7ec2bb51-b527-11e3-a5e2-0800200c9a66"}, //Camlorn Sentinel
            {"3c49795d-24e6-ab02-fe7f-49e31630cd60","5e26d8d3-f451-11e4-b939-0800200c9a66"}, //Camoran Scout Leader
            {"1fc4e1f6-9006-2745-158b-d8f6a6647aa1","beadd0c0-4596-11e3-8f96-0800200c9a66"}, //Cast Out
            {"49738e8d-9833-abd0-79ec-a76b87227e31","33acb17d-300a-11e5-a2cb-0800200c9a66"}, //Cathay-raht Veteran
            {"528842a9-f9f2-0bb7-ca44-6e8d579516b1","0dadeafa-0efb-11e5-b939-0800200c9a66"}, //Cave Grahl
            {"9ff1357e-ce69-a9e1-3935-7cd62585a19a","7e686210-20ce-11e5-867f-0800200c9a66"}, //Chaurus Reaper
            {"1fd87abf-4fdb-4141-6ca7-ffb9acabfacc","0e06fa53-1112-11e5-b939-0800200c9a66"}, //Cheydinhal Sapper
            {"81dbe7a8-56cb-19d4-8b5d-aab135411ba6","90431f3d-94e0-11e3-baa8-0800200c9a66"}, //Chieftain's Banner
            {"09860a4e-63dd-e3ee-c295-94e94745a22a","fbcc37e0-78b7-11e3-981f-0800200c9a66"}, //Child of Hircine
            {"c087de0b-bb21-ea87-9569-b62fd2391080","20b9b7e5-66b6-11e3-949a-0800200c9a66"}, //Cliff Racer
            {"36237bad-224d-f1e3-e05a-4e693b86ecd4","c675aec6-9b9d-11e6-9f33-a24fc0d9649c"}, //Close Call
            {"f5e33229-4ff9-d51e-40c6-f39fba209d4f","51f31703-300a-11e5-a2cb-0800200c9a66"}, //Cloudrest Illusionist
            {"7fd9ae3b-8ccb-3dfb-d0d9-6dbb37559882","90431f42-94e0-11e3-baa8-0800200c9a66"}, //Covenant Marauder
            {"82c28f79-ffdd-3a37-5b96-6213d9b0b7f3","00f37255-52f0-11e3-8f96-0800200c9a66"}, //Craglorn Scavenger
            {"b0ddeaa1-3140-b5be-08f2-56b5860765fc","7e686203-20ce-11e5-867f-0800200c9a66"}, //Crown Quartermaster
            {"eaeb5e3c-1b56-0e61-b252-874d7c5360b5","51f3170b-300a-11e5-a2cb-0800200c9a66"}, //Cruel Firebloom
            {"c49a4131-6a93-7973-6d57-8f54f79aa658","086621f9-ff5d-11e4-b939-0800200c9a66"}, //Crushing Blow
            {"5809dc3c-a869-2ecb-80f9-b30c0c0c84f2","00f37240-52f0-11e3-8f96-0800200c9a66"}, //Crystal Tower Crafter
            {"aac702af-dc1d-3aa7-8e77-e811c4039333","51f31710-300a-11e5-a2cb-0800200c9a66"}, //Cunning Ally
            {"f0ac06aa-7f97-c2ee-a3f5-a7a1f9d79ef5","69e88f6b-87a1-11e3-baa7-0800200c9a66"}, //Curse
            {"7b5ca149-74ea-118c-25fb-6c4b0af39bf2","fbcc37eb-78b7-11e3-981f-0800200c9a66"}, //Cursed Spectre
            {"cbc62693-b504-ecb4-0df3-6263c7540aa7","3cdeb5df-8f61-4eb9-919f-f0b8d61e7663"}, //Daedric Dagger
            {"e3160960-08bb-bb60-a177-994cc71a3598","fbcc5f00-78b7-11e3-981f-0800200c9a66"}, //Daggerfall Mage
            {"82a5a3ec-0a24-3c1b-42eb-844848f8fe6c","beaff3a8-4596-11e3-8f96-0800200c9a66"}, //Dagi-raht Mystic
            {"1e4ca945-78ff-2a80-07ff-e5a35890e80a","8e0d6f6e-f400-11e4-b939-0800200c9a66"}, //Daring Cutpurse
            {"6c894dd5-5e63-d965-e62a-ea55c13acded","51f31707-300a-11e5-a2cb-0800200c9a66"}, //Dark Harvester
            {"c1ba6850-78ea-bfed-2549-e775afe6ec92","e2a07ba2-b3a9-11e3-a5e2-0800200c9a66"}, //Dark Rift
            {"d9ea52b6-1d12-e9fe-e09b-4d410464c2b0","aab8e4bc-f7fc-11e3-a3ac-0800200c9a66"}, //Dawnbreaker
            {"5519cfce-5e20-5a4b-a643-2ee2a480ea78","23d6e31a-4abb-4b4c-9a7b-f404572fbce6"}, //Dawn's Wrath
            {"bc0efe8b-98ae-d9d7-14f6-d235b6880eb3","00f39954-52f0-11e3-8f96-0800200c9a66"}, //Dawnstar Healer
            {"de805806-1c16-190e-86b3-d1c1aae90006","7e68620c-20ce-11e5-867f-0800200c9a66"}, //Deadly Draugr
            {"1beab957-2573-6a3b-fdd6-864312dbf3d4","05b6bab1-ac2e-4c95-8911-d99cd6244610"}, //Deathless Draugr
            {"4242e23f-de96-fbf1-257a-ceb2b639b0eb","8e0d6f6b-f400-11e4-b939-0800200c9a66"}, //Descendant of Alkosh
            {"cb077f5c-103e-ed33-3aee-37dfe20017db","5e26d8da-f451-11e4-b939-0800200c9a66"}, //Deshaan Avenger
            {"64d787fc-5d91-70c7-c6c0-ef907b695dac","5e26d8d2-f451-11e4-b939-0800200c9a66"}, //Deshaan Sneak
            {"df24c0e0-4bf9-fb75-1493-36d51591e1b2","20b9b7e6-66b6-11e3-949a-0800200c9a66"}, //Desperate Conjuring
            {"044cc748-5dab-1d0a-4b8c-ff6400c27907","fd235b5c-0afb-11e5-b939-0800200c9a66"}, //Detain
            {"15a1beac-ea3a-667d-398d-d4e28649ef3d","90431f32-94e0-11e3-baa8-0800200c9a66"}, //Disciple of Namira
            {"7f25fccf-006c-3ebc-810f-eb93aea9c06f","51f31713-300a-11e5-a2cb-0800200c9a66"}, //Divayth Fyr
            {"50796fed-e294-64a9-e395-cda65033ecc2","30dcbb1e-21f1-41ab-bdca-f364224e92d5"}, //Divine Conviction
            {"84d787ac-a857-b8b3-807d-06bc9bcba987","a3e304ff-43cc-4360-b10f-5afe9acd4f62"}, //Divine Fervor
            {"9f2568fb-8fc7-6048-10df-8276afedd747","beb041c2-4596-11e3-8f96-0800200c9a66"}, //Doomcrag Vampire
            {"11977dd5-a900-6d33-76cf-5c9c0f720713","441521f3-300a-11e5-a2cb-0800200c9a66"}, //Dragonstar Rider
            {"3067198f-b2f4-60b2-d53e-794256de5c9f","00f37252-52f0-11e3-8f96-0800200c9a66"}, //Dragontail Savior
            {"902e422c-d1cc-2887-2276-05ce830c954e","8e0d6f62-f400-11e4-b939-0800200c9a66"}, //Dread Clannfear
            {"5de68a1f-6ff6-fa3d-d200-18df8313f1c7","fd235b58-0afb-11e5-b939-0800200c9a66"}, //Dremora Markynaz
            {"337e6735-79da-baf0-71aa-5806624f158a","0dadeaf7-0efb-11e5-b939-0800200c9a66"}, //Dres Guard
            {"f87722ab-c3b5-e1d0-d71d-dc02272da28d","0dadeaf6-0efb-11e5-b939-0800200c9a66"}, //Dres Renegade
            {"1e734f21-0b06-1be6-c4cd-7a873ddeddb8","90431f36-94e0-11e3-baa8-0800200c9a66"}, //Dres Tormentor
            {"38239a59-ed5c-7972-c4c4-f1fc513a5314","0cd25647-e227-11e3-8b68-0800200c9a66"}, //Dreugh Shell Armor
            {"86ed0e94-bd7f-fde8-2149-28909c5a7c6c","0dadeaf0-0efb-11e5-b939-0800200c9a66"}, //Dune Rogue
            {"4fe57f40-bc43-473c-41d4-54d7b98852ec","630b3d42-8f17-4f67-9d2b-9ce98aefddbf"}, //Dune Smuggler
            {"1d970c45-07e3-a4aa-8b83-ff1a10fe00e2","69e88f63-87a1-11e3-baa7-0800200c9a66"}, //Dune Stalker
            {"bc5014e8-3b43-b8c9-d97d-3a76c4c7548b","26a35f04-a6f9-4e90-8db8-c5688057ee61"}, //Dunmer Nightblade
            {"308865bc-e419-3de5-3503-429d98c736c2","0cd2564a-e227-11e3-8b68-0800200c9a66"}, //Dwarven Armaments
            {"12ec02ca-b7c2-c6ec-d1bc-c0d7da80f803","beb041d7-4596-11e3-8f96-0800200c9a66"}, //Dwarven Ballista
            {"71f4b35f-92ba-7a3a-af10-95809ec06932","fbcc5ef4-78b7-11e3-981f-0800200c9a66"}, //Dwarven Centurion
            {"d260eb5d-aac1-0fa2-5815-1ebe6e2e709e","fbcc5ef6-78b7-11e3-981f-0800200c9a66"}, //Dwarven Sphere
            {"96255879-6959-31e5-c017-c0811af4632b","beb041d4-4596-11e3-8f96-0800200c9a66"}, //Dwarven Spider
            {"b7beeed2-b91c-12b0-0e9b-70c2bd77450b","beafcc91-4596-11e3-8f96-0800200c9a66"}, //Earthbone Spinner
            {"29231f22-7d9b-461d-57a9-c13f260ca0a0","7e686209-20ce-11e5-867f-0800200c9a66"}, //Eastmarch Crusader
            {"c37abb3f-5b5f-50cd-7a26-ac14a183f766","fa4ffc66-d4b2-11e3-9c1a-0800200c9a66"}, //Edict of Azura
            {"112bedd3-9640-c291-eccc-2dcee1d434df","7e6861f8-20ce-11e5-867f-0800200c9a66"}, //Elder Centaur
            {"83648f14-ea78-37aa-d4fd-695f8b870b56","69e8b675-87a1-11e3-baa7-0800200c9a66"}, //Elixir of Conflict
            {"b236f408-93a4-465a-9c60-1d6aabeb23be","69e8b673-87a1-11e3-baa7-0800200c9a66"}, //Elixir of Deflection
            {"c95c9dc3-4bdd-294e-b5be-2efa45484252","69e88f67-87a1-11e3-baa7-0800200c9a66"}, //Elixir of Light Feet
            {"94aea99b-d250-8d85-54e7-7dec7c52604e","69e8b682-87a1-11e3-baa7-0800200c9a66"}, //Elixir of the Defender
            {"fce7a985-073b-f9c4-155e-bb1f4b244f08","69e8b676-87a1-11e3-baa7-0800200c9a66"}, //Elixir of Vigor
            {"b40a3cf3-89a0-0c6e-aeb5-0062ebc05203","f66c06c7-2c1c-11e6-bdf4-0800200c9a66"}, //Elsweyr Lookout
            {"4f130569-3dfb-d731-04b6-4ef74e9c8397","aaddc680-9830-11e3-a5e2-0800200c9a66"}, //Elusive Schemer
            {"924fa8fc-b93a-700e-95f3-3eca843a59a5","69e8b677-87a1-11e3-baa7-0800200c9a66"}, //Enchanted Plate
            {"a88d18b4-5aed-93bd-756d-31846bcae761","33acb17c-300a-11e5-a2cb-0800200c9a66"}, //Enraged Mudcrab
            {"36786a4c-f081-5aed-abcd-cb28767b7eaa","00f3723b-52f0-11e3-8f96-0800200c9a66"}, //Evermore Steward
            {"cbd30cec-fe91-b982-501f-62a528eb07a9","13794be2-c4e8-11e3-9c1a-0800200c9a66"}, //Execute
            {"377382b7-17c5-b48d-5942-12cf0c57e7db","e967a7f5-a458-11e5-a837-0800200c9a66"}, //Expert Atromancer
            {"dcc9e8ae-e4cb-2cf7-1558-8f8dae13b991","3d8ab592-b390-11e3-a5e2-0800200c9a66"}, //Falinesti Reaver
            {"697a00e8-3b77-7bb0-691e-eae07c7cc656","6e151041-f451-11e4-b939-0800200c9a66"}, //Farsight Nereid
            {"0808ec9e-1f2c-948e-c5b6-6a1917be02f8","beaff3c4-4596-11e3-8f96-0800200c9a66"}, //Fate Weaver
            {"58c16756-f118-bcac-b550-dccf982aef65","e967a7f2-a458-11e5-a837-0800200c9a66"}, //Fate's Witness
            {"f5643b08-f2b6-d1c9-9e7b-f5e9d9aeaaab","fd235b56-0afb-11e5-b939-0800200c9a66"}, //Fearless Northlander
            {"40327a52-717a-614f-f0a9-ac41f044235c","33acb180-300a-11e5-a2cb-0800200c9a66"}, //Feasting Vulture
            {"62378cd0-7e88-3db6-1441-732111e973f0","33acb179-300a-11e5-a2cb-0800200c9a66"}, //Ferocious Dreugh
            {"34fcdaf7-4958-abec-1f3c-e06ce28f021f","beb01ac7-4596-11e3-8f96-0800200c9a66"}, //Fharun Defender
            {"22488ca5-0eec-98f0-8325-109cb13836b5","beafcc92-4596-11e3-8f96-0800200c9a66"}, //Fiery Imp
            {"a80e5e15-df75-7e5c-cde6-b9e74951fa5c","beb01ab7-4596-11e3-8f96-0800200c9a66"}, //Fifth Legion Trainer
            {"c358d9f8-df93-966d-b54b-66de10f9310f","d0d4131e-a8a0-11e3-a5e2-0800200c9a66"}, //Fighters Guild Recruit
            {"30fb7e72-3e4c-b9f1-6c43-55cb59123561","13794be1-c4e8-11e3-9c1a-0800200c9a66"}, //Finish Off
            {"9942f13d-7712-ad33-a402-4a3cf1637a58","0cd25643-e227-11e3-8b68-0800200c9a66"}, //Fire Storm
            {"622ffe5d-48f1-10f3-edc0-41119f0910bb","69e8b67c-87a1-11e3-baa7-0800200c9a66"}, //Fireball
            {"e1052ede-ffe4-c848-5d1f-9248d001d52b","e574e540-45b4-11e3-8f96-0800200c9a66"}, //Firebolt
            {"d36fa426-f792-5e44-7cb1-1dae3c93d74b","3d8ab590-b390-11e3-a5e2-0800200c9a66"}, //Flesh Atronach
            {"8c917665-c7e0-ae6a-8861-a728bc24def0","c67594cc-9b9d-11e6-9f33-a24fc0d9649c"}, //Forsaken Champion
            {"d1503b6f-48b7-e043-57ba-a45033c056a7","086621fd-ff5d-11e4-b939-0800200c9a66"}, //Forsworn Guide
            {"7a05a977-bf98-9174-0f04-054de85363b0","beadf7d0-4596-11e3-8f96-0800200c9a66"}, //Fortress Watchman
            {"f0d38f2c-54e1-3e94-e5b3-070b54277a88","086621fe-ff5d-11e4-b939-0800200c9a66"}, //Frenzied Witchman
            {"67632e0d-3acd-8bfb-ac85-ca9e9a4dc041","fbcc37e2-78b7-11e3-981f-0800200c9a66"}, //Frostbite Spider
            {"b1cfb66d-3a00-c877-a5f1-f6a0165b588b","87e07786-4f58-11e6-beb8-9e71128cae77"}, //Gardener of Swords
            {"c4b9f457-1182-2723-cfe7-66eeb93ef6ba","086621f2-ff5d-11e4-b939-0800200c9a66"}, //General Tullius
            {"afe095fd-0b41-c8b0-90d4-019fdd17b565","beafccac-4596-11e3-8f96-0800200c9a66"}, //Giant Bat
            {"eed59b80-0813-5e82-b5a4-a8b810f6e4d1","69e8b678-87a1-11e3-baa7-0800200c9a66"}, //Giant Snake
            {"4d9a781e-f63a-5a70-1abe-4e8a91921afe","aab8e4ba-f7fc-11e3-a3ac-0800200c9a66"}, //Gladiator Arena
            {"919775ec-9e51-042a-e985-41c13570a774","0356f011-ff60-11e4-b939-0800200c9a66"}, //Glenumbra Sorceress
            {"2af0cfc8-3008-38bd-1ed5-2b8d36cb4b84","99c98996-b3a3-11e3-a5e2-0800200c9a66"}, //Gloom Wraith
            {"76201e7a-e46a-151f-df84-9992aac25ae2","69e88f62-87a1-11e3-baa7-0800200c9a66"}, //Goblin Skulk
            {"c6f1f39c-14f5-11ba-0409-502de4aa11a7","90431f48-94e0-11e3-baa8-0800200c9a66"}, //Goldbrand
            {"702a50cb-695c-8bf8-eca6-9b6290bfa252","3a751803-e810-11e3-ac10-0800200c9a66"}, //Golden Saint
            {"a7c7ddd8-c367-d4d0-baaa-4cd4a837a485","00f3724c-52f0-11e3-8f96-0800200c9a66"}, //Gortwog gro-Nagorm
            {"df77b37a-0f99-0389-299e-edbdc3ac4697","d0d41300-a8a0-11e3-a5e2-0800200c9a66"}, //Grahtwood Ambusher
            {"08af31c9-d78b-aa05-8acd-8bfac0d1789e","112be60e-e814-4d62-96ad-4fa1258c4f7a"}, //Graystone Ravager
            {"18d9ae9a-b748-feee-9674-9fc8b6eefa6d","85e58240-1143-11e5-b939-0800200c9a66"}, //Green Pact Stalker
            {"9fe77d8d-9e66-2a88-4adb-51a27eea5873","85e58242-1143-11e5-b939-0800200c9a66"}, //Greenheart Knight
            {"8bcdada2-04d6-0925-c1c4-420119b1631f","51f31715-300a-11e5-a2cb-0800200c9a66"}, //Green-Touched Spriggan
            {"fa50d637-13fe-6e29-9eaf-f1d08c8b908d","69e88f61-87a1-11e3-baa7-0800200c9a66"}, //Grim Champion
            {"2b54b76e-76d2-7a25-925f-c8b0db06bfca","fd235b5e-0afb-11e5-b939-0800200c9a66"}, //Guild Recruit
            {"3de2cfcd-174e-6296-ad43-a9b3cd3fa340","6e151042-f451-11e4-b939-0800200c9a66"}, //Haafingar Marauder
            {"17767ab2-292e-86ec-5a4b-057731ad0f3e","0cd2564c-e227-11e3-8b68-0800200c9a66"}, //Hackwing Feather
            {"2117593f-819b-91ef-f0a7-c74da3e7f098","47c61fa0-b529-11e3-a5e2-0800200c9a66"}, //Halls of the Dwemer
            {"98589bce-cf3e-4677-f863-a4df71a53f8a","0e06fa5b-1112-11e5-b939-0800200c9a66"}, //Haunting Spirit
            {"6b99cb3d-25a0-bb79-29cd-b169754086f2","441521f0-300a-11e5-a2cb-0800200c9a66"}, //Healing Hands
            {"8c566885-c733-0c14-3547-5f1f3cb192cf","90431f43-94e0-11e3-baa8-0800200c9a66"}, //Healing Potion
            {"9c0e965e-53cd-21b1-7544-5d2755f02529","fbcc37f7-78b7-11e3-981f-0800200c9a66"}, //Heavy Battleaxe
            {"d773e6e8-e309-e9b8-3d23-f68a63acbed6","d0d3ebf7-a8a0-11e3-a5e2-0800200c9a66"}, //Heirloom Greatsword
            {"b9d9519e-c647-bc0a-946f-3ab042a5ca50","beb01ab6-4596-11e3-8f96-0800200c9a66"}, //Helgen Squad Leader
            {"26a3b7ec-2b0c-fc99-dc15-aef01bbef6a2","beb01ac5-4596-11e3-8f96-0800200c9a66"}, //Helstrom Footpad
            {"b9644e8f-f6f8-f9eb-2c4f-4b8acb6c0255","90431f35-94e0-11e3-baa8-0800200c9a66"}, //Hero of Anvil
            {"1ccaad69-ab04-dfd2-4c81-dc4a970a3aca","4c7d31b0-c22c-11e6-a4a6-cec0c932ce01"}, //Heroic Rebirth
            {"14821ec2-29d3-f89a-acfd-e0b354df4151","5e26d8d7-f451-11e4-b939-0800200c9a66"}, //Hidden Trail
            {"2f0a6ea0-7fe0-93bb-9958-781174559847","13794be0-c4e8-11e3-9c1a-0800200c9a66"}, //High King Emeric
            {"393d5699-763d-7cb1-0cae-bf615cbbf3cd","beaff3a9-4596-11e3-8f96-0800200c9a66"}, //High Rock Summoner
            {"264ce261-0994-c922-95bd-8fd208cf1207","7e686211-20ce-11e5-867f-0800200c9a66"}, //Highland Lurcher
            {"f46113f1-62a4-ad73-09c8-6854d7d7c1b1","ef3afb02-dfdb-11e5-a837-0800200c9a66"}, //Hist Grove
            {"12ce0c15-c941-9ca5-d5b5-565b05f80471","beb041c6-4596-11e3-8f96-0800200c9a66"}, //Hist Speaker
            {"4feef51e-f469-5b94-c09f-60cbdf1bf402","fbcc5efa-78b7-11e3-981f-0800200c9a66"}, //Hive Defender
            {"587c7b3d-b986-6a47-4e1a-98e80ce257c7","fbcc5efb-78b7-11e3-981f-0800200c9a66"}, //Hive Warrior
            {"0bcbc507-5417-2127-1abe-43e22d06695a","fbcc5ef9-78b7-11e3-981f-0800200c9a66"}, //Hive Worker
            {"bac505af-406b-4bce-2a5f-d58d6c355e89","5cdd075c-45e7-4b1f-9844-97488cb3b789"}, //House Kinsman
            {"72249ee8-f927-5de0-3b17-63194e9e8583","51f31705-300a-11e5-a2cb-0800200c9a66"}, //Ice Spike
            {"2608d907-8952-fb6f-c73c-65a11f2d7826","00f37237-52f0-11e3-8f96-0800200c9a66"}, //Ice Storm
            {"6fab1a25-0e5d-3d43-ef4e-079abc243ae5","00f39944-52f0-11e3-8f96-0800200c9a66"}, //Ice Wraith
            {"5c7fd632-9e4b-e426-9bbf-08904f22086e","00f37254-52f0-11e3-8f96-0800200c9a66"}, //Iliac Sorcerer
            {"3c9bdcc6-212a-0f59-2828-c7a9cb7e46eb","c675a638-9b9d-11e6-9f33-a24fc0d9649c"}, //Illusory Mimic
            {"9b53db1b-6a55-ef2d-78eb-2595a0933281","51f31704-300a-11e5-a2cb-0800200c9a66"}, //Immolating Blast
            {"be4c3fdf-7395-aebd-b2c0-dd604e98d583","d0d41307-a8a0-11e3-a5e2-0800200c9a66"}, //Imperial Armor
            {"9b8bd8f8-d22d-e9bb-7218-de464b8fffb0","d77dcaf7-cad1-4125-8eba-cda9b6f3bcb5"}, //Imperial Legionnaire
            {"76094dea-fbd6-d306-eaaa-16c5e15b6d9f","d0d4130a-a8a0-11e3-a5e2-0800200c9a66"}, //Imperial Might
            {"878d61d8-95b4-2bb3-f949-447828624e75","beb01abc-4596-11e3-8f96-0800200c9a66"}, //Imperial Reinforcements
            {"68e65c5c-a2c7-5f87-4a9a-1775a5524ecb","33acb183-300a-11e5-a2cb-0800200c9a66"}, //Imperial Siege Engine
            {"3cdfd8d7-1678-ac0d-acb6-5a9b3c88dc70","04bffff3-673b-11e3-949a-0800200c9a66"}, //Imprison
            {"1a54881b-f292-dfb9-4ace-c7dd355c9712","fa4ffc64-d4b2-11e3-9c1a-0800200c9a66"}, //Imprisoned Deathlord
            {"a4f3e1f1-08e8-5de5-368b-fc0bd1656ea6","51f31702-300a-11e5-a2cb-0800200c9a66"}, //Improvised Weapon
            {"a05697fa-29ed-6e07-4440-c0a0935e3aaf","00f39945-52f0-11e3-8f96-0800200c9a66"}, //Indoril Archmage
            {"faafbee0-ae41-b0af-f5cd-d2c95982f521","33acb175-300a-11e5-a2cb-0800200c9a66"}, //Initiate of Hircine
            {"d0761488-7d73-d263-15ef-464dff6dd254","beadd0c2-4596-11e3-8f96-0800200c9a66"}, //Intimidate
            {"3fd0407a-4069-3edd-fcac-64c26ab1c920","de25f383-7a23-11e3-981f-0800200c9a66"}, //Iron Atronach
            {"635d3fc9-0b45-42af-2422-2572aa41fe5f","d0d41313-a8a0-11e3-a5e2-0800200c9a66"}, //Jerall Forager
            {"880f905a-03d0-1a15-3966-3d52cb303d14","3a751805-e810-11e3-ac10-0800200c9a66"}, //Keeper of Whispers
            {"e23f120d-29f9-6b6a-aec9-aa6060acb4b5","1b1043f0-e5bc-11e3-ac10-0800200c9a66"}, //Kvatch Soldier
            {"67f3d191-bb0a-2d00-02a1-63994c1d4cf3","85e58243-1143-11e5-b939-0800200c9a66"}, //Leaflurker
            {"7876313e-95e4-99cb-eaab-1d9d14eb6449","de25cc72-7a23-11e3-981f-0800200c9a66"}, //Leafwater Blessing
            {"e04b8afa-a724-1a01-eca3-51cf59a929af","6a819e55-10e2-4d4c-b0b0-c618871c064c"}, //Legion Praefect
            {"d5a3965f-28ff-3760-b9a7-33a2e5f15bf7","beaff3cb-4596-11e3-8f96-0800200c9a66"}, //Legion Shield
            {"883566ba-943e-e52f-c9f0-041ae5846cb5","fbcc37e8-78b7-11e3-981f-0800200c9a66"}, //Lesser Ward
            {"74df08b7-815a-6b6f-0ef4-edba1a9f5c7e","beadd0ca-4596-11e3-8f96-0800200c9a66"}, //Lightning Bolt
            {"9344c1d7-981e-cb15-a29e-a567f66f8e91","fbcc5ef8-78b7-11e3-981f-0800200c9a66"}, //Lillandril Hexmage
            {"05618f90-2c51-a31e-3e5a-737ec8e14ae8","086621f6-ff5d-11e4-b939-0800200c9a66"}, //Lion Guard Strategist
            {"319fbc49-7bc7-6abe-0d92-3e20d3257bc1","fbcc5ef3-78b7-11e3-981f-0800200c9a66"}, //Lowland Troll
            {"f634c259-e501-f5a3-70eb-2d68192e67dd","69e8b680-87a1-11e3-baa7-0800200c9a66"}, //Loyal Housecarl
            {"cba167eb-2160-cb86-1251-b8808c94c085","33acb17f-300a-11e5-a2cb-0800200c9a66"}, //Lucien Lachance
            {"6aececba-9f30-2492-9b25-32a19e913b19","441521f4-300a-11e5-a2cb-0800200c9a66"}, //Lumbering Ogrim
            {"0430649d-71eb-04e6-580f-e6996d92f4ff","62a40f7a-5be2-4ee6-b8fe-47678157e81b"}, //Lurking Crocodile
            {"daf74f7d-6d31-1849-0d01-8a1bd1d1585e","075d555c-5c24-4fdc-9779-029cd03842c1"}, //Lurking Mummy
            {"3f32a8ae-f7e3-da42-4ca0-937dcbe28e0e","aab8e4b4-f7fc-11e3-a3ac-0800200c9a66"}, //Mace of Encumbrance
            {"c90316fa-431f-298d-e586-e537815cab56","8e0d6f60-f400-11e4-b939-0800200c9a66"}, //Mage Slayer
            {"f5063ae9-1286-d06a-de61-7d28c08ba644","d0d3ebf8-a8a0-11e3-a5e2-0800200c9a66"}, //Mages Guild Retreat
            {"17be27cd-136c-fdd5-572a-e2bca565d1fb","5e26d8d4-f451-11e4-b939-0800200c9a66"}, //Malefic Wreath
            {"f01aae34-4963-8983-aa07-0a3e456d2a68","d9087250-a8a0-11e3-a5e2-0800200c9a66"}, //Mantikora
            {"06563d11-95e5-03dc-f54f-88cac61f7a9f","69e8b674-87a1-11e3-baa7-0800200c9a66"}, //Maple Shield
            {"49661a9c-a70b-7017-f83e-c7b807fc6d6e","beafcc94-4596-11e3-8f96-0800200c9a66"}, //Markarth Bannerman
            {"782633a2-4159-379e-6062-d4417a65de81","20b9dee9-66b6-11e3-949a-0800200c9a66"}, //Master of Arms
            {"11391d9a-adf6-2e45-e223-be79a5d67cde","8e0d6f6d-f400-11e4-b939-0800200c9a66"}, //Master of Thieves
            {"0809f2c0-e537-8201-14c5-ecd90ea7b6c5","f66c06c0-2c1c-11e6-bdf4-0800200c9a66"}, //Mechanical Ally
            {"8c40f677-1521-3806-0c43-0e8ab839362e","6e151043-f451-11e4-b939-0800200c9a66"}, //Mentor's Ring
            {"cd8efed9-dc06-ba4d-d751-af1c157493b3","345a4d52-d3a7-48ef-a65d-8dd4fc713606"}, //Merchant's Camel
            {"73b79b6e-ada7-dded-3419-ec30577078a4","086621f8-ff5d-11e4-b939-0800200c9a66"}, //Merric-at-Aswala
            {"0167f613-74c8-8279-1f73-9253992e672f","beb01ab2-4596-11e3-8f96-0800200c9a66"}, //Midnight Sweep
            {"7f9e5ded-5d80-f616-05fd-fdf53f73124c","51f3170d-300a-11e5-a2cb-0800200c9a66"}, //Mighty Ally
            {"9d28fee3-c880-38fe-9db1-110d5584ed4d","20b9b7e3-66b6-11e3-949a-0800200c9a66"}, //Militant Chieftain
            {"91e3453c-628b-54b8-00d0-fcbb68446e1b","00f37244-52f0-11e3-8f96-0800200c9a66"}, //Miraak, Dragonborn
            {"cfacf55d-3767-91d3-ea9f-96d18aee690e","0e06fa58-1112-11e5-b939-0800200c9a66"}, //Moment of Clarity
            {"4c4f596e-7d50-899b-a350-4562491f6754","fd235b55-0afb-11e5-b939-0800200c9a66"}, //Moonlight Werebat
            {"9352b699-cafd-7bbc-7b8b-c33629d68f0c","08662201-ff5d-11e4-b939-0800200c9a66"}, //Morkul Gatekeeper
            {"60e41349-7214-cbdf-9dc8-61406223d802","beb01ab5-4596-11e3-8f96-0800200c9a66"}, //Morthal Executioner
            {"0d1bac72-c9c6-78be-017c-7c7d857e1349","f6f89c10-ffe1-11e4-b939-0800200c9a66"}, //Morthal Watchman
            {"7647fe4a-c6ca-7c01-67d7-1add7e0fac59","fa526153-241a-11e4-8c21-0800200c9a66"}, //Mountain Tyrant
            {"cf98edea-b8ca-0b5c-0a7a-2bbc246a0994","beb01ac8-4596-11e3-8f96-0800200c9a66"}, //Mournhold Guardian
            {"4e83304e-0795-e5ae-7c9d-bc08b1eb0b28","33acb182-300a-11e5-a2cb-0800200c9a66"}, //Mournhold Traitor
            {"4837c3f0-07f6-bed5-1c6d-cac70e3378b2","20b9b7d3-66b6-11e3-949a-0800200c9a66"}, //Mummify
            {"c85fbd88-39ca-950b-ddbf-88e2142dad71","99c98990-b3a3-11e3-a5e2-0800200c9a66"}, //Mundus Stone
            {"68e16a20-2a56-89ed-f258-62e044d1771d","20b9b7d5-66b6-11e3-949a-0800200c9a66"}, //Murkwater Butcher
            {"de60c31c-5eb7-eafc-8f30-80fef0d540cb","d0d41314-a8a0-11e3-a5e2-0800200c9a66"}, //Murkwater Goblin
            {"2274594f-3d71-c36a-9b1c-91c0d44df5a5","beb01acb-4596-11e3-8f96-0800200c9a66"}, //Murkwater Savage
            {"f6868917-b840-ce52-bed9-bceefd549611","5e26d8d9-f451-11e4-b939-0800200c9a66"}, //Murkwater Shaman
            {"9dfb823c-e929-72af-ada1-cd7ab61dc75b","5e26d8db-f451-11e4-b939-0800200c9a66"}, //Murkwater Skirmisher
            {"b1f55e61-d29b-e3d7-e7a4-581a102e0b91","beb01aca-4596-11e3-8f96-0800200c9a66"}, //Murkwater Witch
            {"c5ba4bf5-0769-275d-0a9d-ea5afc3d1895","beaff3ab-4596-11e3-8f96-0800200c9a66"}, //Nahagliiv
            {"de0e5c4d-c486-150f-8d40-2376ae6dc702","aab8e4b5-f7fc-11e3-a3ac-0800200c9a66"}, //Nahkriin, Dragon Priest
            {"c9a32e5e-83b8-b20a-8950-1c1d77d835d2","51f31700-300a-11e5-a2cb-0800200c9a66"}, //Necrom Mastermind
            {"82dc190e-d554-2a37-dd02-7581db3a2f8c","62bd16c5-61b7-4074-9d2f-fd771f883c6c"}, //Necromancer's Amulet
            {"6bf41248-80cf-5db5-8256-954db83d8df9","51f31712-300a-11e5-a2cb-0800200c9a66"}, //Nest of Vipers
            {"db80b123-73f1-c2cf-a55c-47d4371ac61b","00f3723c-52f0-11e3-8f96-0800200c9a66"}, //Niben Bay Cutthroat
            {"08f92b77-2d7c-b7e1-9563-ce00d81206c5","7e686201-20ce-11e5-867f-0800200c9a66"}, //Night Patrol
            {"d2889d2a-72c0-e7fa-b4bc-41ba43e1ade9","00f37247-52f0-11e3-8f96-0800200c9a66"}, //Night Predator
            {"8e1cc1e9-6c1d-49e9-0979-e0487dee9d01","7e686202-20ce-11e5-867f-0800200c9a66"}, //Night Shadow
            {"f472de00-c839-de74-315b-211197a0cb67","7e6861fc-20ce-11e5-867f-0800200c9a66"}, //Night Talon Lord
            {"6d896056-05f6-b5b1-1ff9-d132899fb724","51f3170c-300a-11e5-a2cb-0800200c9a66"}, //Nimble Ally
            {"4bb5254e-7cb0-6f0f-3907-313cfb2502a0","0cd25652-e227-11e3-8b68-0800200c9a66"}, //Nord Firebrand
            {"4e4b1909-61bf-d3d2-4e28-846fe7be30d2","beaff3a7-4596-11e3-8f96-0800200c9a66"}, //Northpoint Captain
            {"da2087da-272b-1575-58f9-893e33781e08","0e06fa51-1112-11e5-b939-0800200c9a66"}, //Northpoint Lieutenant
            {"158e9d07-7b24-3632-95bf-fbafbe6634c9","99c98997-b3a3-11e3-a5e2-0800200c9a66"}, //Northwind Outpost
            {"5870bcfa-af97-86b8-642b-143e6ed2f73b","33acb174-300a-11e5-a2cb-0800200c9a66"}, //Odahviing
            {"2b655cc5-a7db-d1ff-943b-faa2100e3caf","beafccb8-4596-11e3-8f96-0800200c9a66"}, //Oldgate Warden
            {"1f389ae6-206e-c73e-b15a-fb87565110c4","e967a7f4-a458-11e5-a837-0800200c9a66"}, //Orb of Vaermina
            {"01e79292-074f-76ff-5054-ef7bd2222aff","beadd0c9-4596-11e3-8f96-0800200c9a66"}, //Orc Clan Captain
            {"fa38f95d-4a00-aa81-7f4f-b960fc867539","0e06fa54-1112-11e5-b939-0800200c9a66"}, //Orc Clan Shaman
            {"b6f487de-7f95-6992-e25e-bfe4f482a32a","631142f2-300a-11e5-a2cb-0800200c9a66"}, //Orc Clansman
            {"9b3d719a-b68d-b07d-73dd-93d56ed7ab48","fbcc37f1-78b7-11e3-981f-0800200c9a66"}, //Orcish Warhammer
            {"b4e893ef-d5cf-eeb3-bba2-93b1c619ec6b","0356f01a-ff60-11e4-b939-0800200c9a66"}, //Pack Wolf
            {"ed87e7c0-8fd4-6384-5a97-922d9b3f0c3d","beb01acc-4596-11e3-8f96-0800200c9a66"}, //Pahmar-raht Renegade
            {"81d12724-5fe4-fe56-cdb2-aa68f8031148","6738ecd8-8cc3-4aef-80a2-4db7feb6ed6f"}, //Piercing Javelin
            {"52b70184-4895-4975-65e5-32b57d514b40","08662204-ff5d-11e4-b939-0800200c9a66"}, //Pillaging Tribune
            {"1e350e47-658e-cbed-09c9-c25499f778a0","fa526154-241a-11e4-8c21-0800200c9a66"}, //Pit Lion
            {"432898a7-6d44-d5c4-f3f5-2b164e340cc8","9d3326b0-f400-11e4-b939-0800200c9a66"}, //Plea to Kynareth
            {"197fcb85-39fc-8bc8-c5da-60c5e59d1ed5","6e153752-f451-11e4-b939-0800200c9a66"}, //Plunder
            {"8a953da1-0085-1d50-77c4-1ab44822e84d","90431f39-94e0-11e3-baa8-0800200c9a66"}, //Portcullis
            {"acd58013-1d8e-49ef-1229-df1cb8384afe","fa4ffc63-d4b2-11e3-9c1a-0800200c9a66"}, //Preserver of the Root
            {"38a1e9c5-e6e6-87b2-dde3-762b2e0259d1","beb01ab9-4596-11e3-8f96-0800200c9a66"}, //Priest of the Moons
            {"2b107ef3-f201-46e1-8a4d-7bdc64bebc92","0e06fa56-1112-11e5-b939-0800200c9a66"}, //Queen Barenziah
            {"fc825917-5466-7016-b903-4b0eadb87cfb","8e0d6f67-f400-11e4-b939-0800200c9a66"}, //Quin'rawl Burglar
            {"f5a464bf-c967-092d-c55a-4df29d39ff0c","7e6861f6-20ce-11e5-867f-0800200c9a66"}, //Quin'rawl Skulker
            {"d4ce4fc4-6dc0-b011-457a-00e0982584ea","0cd25653-e227-11e3-8b68-0800200c9a66"}, //Raiding Party
            {"adbd41e3-9b39-ee0a-7ae6-b10d74d111d3","8e0d6f6a-f400-11e4-b939-0800200c9a66"}, //Rajhini Highwayman
            {"79a73e4d-23b4-75c0-3289-69fb56b4b6a1","beadd0c8-4596-11e3-8f96-0800200c9a66"}, //Rampaging Minotaur
            {"f6456966-6b8a-ba88-234c-1d780bcd6a6c","51f31718-300a-11e5-a2cb-0800200c9a66"}, //Ransack
            {"7461ee12-811c-5478-d40b-1a36217ad834","51f31708-300a-11e5-a2cb-0800200c9a66"}, //Rapid Shot
            {"5f91d030-9496-dae8-6a6c-311b18710b02","086621fc-ff5d-11e4-b939-0800200c9a66"}, //Ravenous Crocodile
            {"5bb06db5-964d-87df-0906-78277341077f","7e686213-20ce-11e5-867f-0800200c9a66"}, //Ravenous Hunger
            {"2b478f5d-fd68-b270-50c0-2457e32e9910","086621f0-ff5d-11e4-b939-0800200c9a66"}, //Reachman Shaman
            {"b1647107-9309-5445-b02e-1c772352b292","631142f0-300a-11e5-a2cb-0800200c9a66"}, //Reclusive Giant
            {"67e14fbf-d7ba-c370-603e-b68458a4f546","0e06fa55-1112-11e5-b939-0800200c9a66"}, //Red Bramman
            {"8983cd35-810c-b063-424f-fe942ce09c62","12df819f-e52c-4b0d-a3f8-ed6029928b11"}, //Redoran Enforcer
            {"d57ce0d0-3314-df93-006b-de72ef75f95a","0356f017-ff60-11e4-b939-0800200c9a66"}, //Reive, Blademaster
            {"3a393624-42d1-eab7-ebce-6cacaa7b56ed","6e153750-f451-11e4-b939-0800200c9a66"}, //Relentless Raider
            {"c9b653ad-f51d-55e6-0d4f-98c907dd3ad5","00f39950-52f0-11e3-8f96-0800200c9a66"}, //Renowned Legate
            {"4180920b-fce7-d139-c224-aebef02a61da","51f3170e-300a-11e5-a2cb-0800200c9a66"}, //Resolute Ally
            {"c25af0b4-4621-a99b-4aae-cc8229c569d6","0e06fa59-1112-11e5-b939-0800200c9a66"}, //Restless Templar
            {"baedda7c-7ff0-b310-3395-67084c9c3c8b","441521f5-300a-11e5-a2cb-0800200c9a66"}, //Rift Thane
            {"066902cd-faae-4b41-1044-b5ffe3197519","8e0d6f63-f400-11e4-b939-0800200c9a66"}, //Riften Pillager
            {"b74ce3d7-62c9-4edb-66f7-987db180ec87","0dadeaf3-0efb-11e5-b939-0800200c9a66"}, //Rihad Battlemage
            {"144c29ac-9515-d1c1-9744-1237e2c21095","0dadeaf4-0efb-11e5-b939-0800200c9a66"}, //Rihad Horseman
            {"bb00d078-2cb8-f4c3-7e73-3b943b32fcd6","0dadeaf5-0efb-11e5-b939-0800200c9a66"}, //Rihad Nomad
            {"728a3472-f34e-94f9-e22e-83c668f9dea5","4efbb258-5db0-11e6-8b77-86f30ca893d3"}, //Ring of Imaginary Might
            {"35e5fb0b-acb7-4638-1e43-234777871949","137972f2-c4e8-11e3-9c1a-0800200c9a66"}, //Rising Legate
            {"4476dbc4-7ff7-59a9-ec78-b728b8eab8b2","8e0d6f66-f400-11e4-b939-0800200c9a66"}, //Riverhold Escort
            {"a8de9008-a2b4-d663-04ea-9a4879b0b406","d0d41311-a8a0-11e3-a5e2-0800200c9a66"}, //Rotting Draugr
            {"44d5b31a-f67f-7b6a-ab03-31f88b6034dc","d0d4130c-a8a0-11e3-a5e2-0800200c9a66"}, //Royal Sage
            {"6c003727-ad2b-6344-e0ef-215ffda471fa","0e06fa57-1112-11e5-b939-0800200c9a66"}, //Sadras Agent
            {"61653fce-e427-7791-3402-ce3eb0851bc4","4fd4ef30-b521-11e3-a5e2-0800200c9a66"}, //Savage Ogre
            {"2f0334b6-36e7-3b35-3b57-9b3a7528d215","de25cc70-7a23-11e3-981f-0800200c9a66"}, //Scouting Patrol
            {"0d8e4a88-c06c-d88c-8fa6-89087a637562","beadd0cf-4596-11e3-8f96-0800200c9a66"}, //Scuttler
            {"a66336f8-eac6-5db8-f45b-5aa021f33536","33acb17e-300a-11e5-a2cb-0800200c9a66"}, //Senche-Tiger
            {"7d6373df-520a-4522-2e83-7f307873d788","51f31706-300a-11e5-a2cb-0800200c9a66"}, //Sentinel Battlemace
            {"efda54db-d074-8bca-c88e-d6520b651ca4","fbcc37e9-78b7-11e3-981f-0800200c9a66"}, //Septim Guardsman
            {"a7b63e2d-0c88-d203-46ad-471af1c3fc7e","51f3170a-300a-11e5-a2cb-0800200c9a66"}, //Shadow Shift
            {"2b94114a-8439-d045-3f12-e77c79d2882b","4cde7090-e289-11e3-8b68-0800200c9a66"}, //Shadowfen Priest
            {"48bf5093-fda3-817c-642d-4a0f51ec4695","5bd5b620-e28f-11e3-8b68-0800200c9a66"}, //Sharpshooter Scout
            {"67f42b2e-030e-9e7a-4513-ed3d75787dbc","0cd25644-e227-11e3-8b68-0800200c9a66"}, //Shimmerene Peddler
            {"595904e7-9eed-c017-cff7-3c3d3e153818","a1e53392-a6a1-4d2c-ae0f-fb7928edffbd"}, //Shivering Apothecary
            {"192c46fe-9ad3-e2b4-714a-4bb6bd848fdb","7e68620e-20ce-11e5-867f-0800200c9a66"}, //Shocking Wamasu
            {"3e4067a6-a5ad-dbe1-51d3-329727486749","beaff3c5-4596-11e3-8f96-0800200c9a66"}, //Shornhelm Champion
            {"3980371e-8e1c-7d8c-eded-64196401a012","aa780142-bebc-44a2-aedc-f3f32277a6c1"}, //Shrieking Harpy
            {"9f06d2b3-c92f-831a-05fa-6affcbd33a2f","69e8b670-87a1-11e3-baa7-0800200c9a66"}, //Siege Catapult
            {"e3cff698-9254-b1c5-c6d3-181ee2441f24","85e58241-1143-11e5-b939-0800200c9a66"}, //Silvenar Tracker
            {"002b7759-dd8c-cbef-0821-2ddbd1ba4db8","51f31709-300a-11e5-a2cb-0800200c9a66"}, //Skaven Pyromancer
            {"ba78d3d8-173a-76d6-bff8-36f9b1701e3c","fbcc37ef-78b7-11e3-981f-0800200c9a66"}, //Skilled Blacksmith
            {"64948944-9f25-1630-6647-b55c6dbaf225","c3d0123c-7538-4319-b3e8-5486b7ff2d40"}, //Skingrad Patroller
            {"5f3af07f-7aeb-9c04-505d-9af48c261289","69e8b67f-87a1-11e3-baa7-0800200c9a66"}, //Skirmisher's Elixir
            {"90ccdc31-48db-7847-198f-1a445a9d89c8","69e88f69-87a1-11e3-baa7-0800200c9a66"}, //Skooma Racketeer
            {"b28bd68a-c91e-f236-6dc1-21e31826ab46","beafcca0-4596-11e3-8f96-0800200c9a66"}, //Skywatch Vindicator
            {"33e3ee04-46d0-ed33-18c4-09fa561d387f","7e68620a-20ce-11e5-867f-0800200c9a66"}, //Slaughterfish
            {"2e53f9c1-4b0f-0ff5-72c7-fc45cf4d3f6f","7e68620b-20ce-11e5-867f-0800200c9a66"}, //Slaughterfish Spawning
            {"79fab68a-cecf-231a-549c-2649d4633a5d","ef3afb05-dfdb-11e5-a837-0800200c9a66"}, //Smuggler's Haul
            {"40cc1a36-2ea9-5288-946f-d309f825a249","d0d41309-a8a0-11e3-a5e2-0800200c9a66"}, //Snake Tooth Necklace
            {"8e940763-947b-9b1d-1969-37518fdd159c","7e68620f-20ce-11e5-867f-0800200c9a66"}, //Snapping Dreugh
            {"dbaf84be-ee84-3428-4d8f-9688d8aa4c09","0e06fa5e-1112-11e5-b939-0800200c9a66"}, //Snow Wolf
            {"f0d9921c-7b01-a7df-d0a3-de382e81559d","7fdfb0b1-153e-11e5-b939-0800200c9a66"}, //Snowy Sabre Cat
            {"c889bc91-6bfe-cb7c-3f83-30437e87afc7","0e06fa5d-1112-11e5-b939-0800200c9a66"}, //Soul Split
            {"63b0d911-5fdc-2c83-d900-dd63448f2538","fd235b54-0afb-11e5-b939-0800200c9a66"}, //Soulrest Marshal
            {"168198e7-5104-f5c3-9f1f-5f626479d91d","4c7d1cca-c22c-11e6-a4a6-cec0c932ce01"}, //Sower of Revenge
            {"273b6c42-b522-6ef2-446e-2d2eede0c146","20b9b7e0-66b6-11e3-949a-0800200c9a66"}, //Sparking Spider
            {"5c682a05-c61c-9a11-c680-f6b2f8a3612a","0cd25641-e227-11e3-8b68-0800200c9a66"}, //Spider Daedra
            {"c3a8bbee-8555-194d-1e35-b7d38a50f25d","0dadeaf1-0efb-11e5-b939-0800200c9a66"}, //Spider Lair
            {"a0e71fa3-ee7a-f1de-90e0-3e0634f8eae0","aab8e4b7-f7fc-11e3-a3ac-0800200c9a66"}, //Spider Worker
            {"af60a453-e0bd-7cc7-48fc-bce4356e83f5","de25f382-7a23-11e3-981f-0800200c9a66"}, //Spiteful Dremora
            {"f1f4dc76-8188-80c3-789a-3995efe87ec6","99c98993-b3a3-11e3-a5e2-0800200c9a66"}, //Staff of Sparks
            {"6413ecd2-81f4-88c7-810b-572011cce09f","0dadeaf9-0efb-11e5-b939-0800200c9a66"}, //Stalking Crocodile
            {"f0584f66-a6bc-7147-efc3-a5605f8f2fa7","51f3170f-300a-11e5-a2cb-0800200c9a66"}, //Stalwart Ally
            {"72ac5adf-fbbf-5442-14cf-ea61ea0ad69d","1232d5de-d468-11e6-bf26-cec0c932ce01"}, //Stampede Sentinel
            {"ea65383d-750c-cf79-8f18-8265b911c025","7fdfd7c0-153e-11e5-b939-0800200c9a66"}, //Stampeding Mammoth
            {"74cf1df3-fe53-93ba-5bba-0d6c0e8dcb09","7e686212-20ce-11e5-867f-0800200c9a66"}, //Starved Hunger
            {"054c81f3-cbd1-44bd-87e1-ec7e8c981158","00f3724b-52f0-11e3-8f96-0800200c9a66"}, //Steel Scimitar
            {"42e39126-215a-0c6d-4be1-2f23c6956479","489c4118-c4dd-4a39-a0f0-14d233880bb2"}, //Steel Sword
            {"57d7d47e-5f6c-ccfd-b12a-1df047a64da6","51f31701-300a-11e5-a2cb-0800200c9a66"}, //Stone Throw
            {"fa22ceed-9c1c-c918-5fec-28eb266b8f08","8df590d6-6ae0-11e6-8b77-86f30ca893d3"}, //Stoneshard Orc
            {"198c1b81-51a2-7239-34f8-075d37570874","00f37246-52f0-11e3-8f96-0800200c9a66"}, //Stonetooth Scrapper
            {"b5af1655-459f-b446-e7b5-a6f3c7c73239","90431f3b-94e0-11e3-baa8-0800200c9a66"}, //Stormhold Henchman
            {"dff472ef-8861-4de8-7cb1-ffe9df8f3b0a","086621f1-ff5d-11e4-b939-0800200c9a66"}, //Stronghold Eradicator
            {"f9c91c24-7000-fe3c-1466-f3746d9a43bf","086621fa-ff5d-11e4-b939-0800200c9a66"}, //Stronghold Incubator
            {"47036d86-53b9-5623-8d85-84148bd705c6","0dadeaf8-0efb-11e5-b939-0800200c9a66"}, //Stronghold Prototype
            {"b2857782-e597-7909-2341-8001b34307d9","33acb17b-300a-11e5-a2cb-0800200c9a66"}, //Student of Arms
            {"dc1d16fe-5eee-de14-c114-94023019ba66","20b9b7df-66b6-11e3-949a-0800200c9a66"}, //Studium Headmaster
            {"d89941ac-f00f-7e19-8389-43633749e205","90431f38-94e0-11e3-baa8-0800200c9a66"}, //Summerset Orrery
            {"178a6bcb-a2df-dd68-f8d4-1529e0198ad0","00f39958-52f0-11e3-8f96-0800200c9a66"}, //Summerset Shieldmage
            {"661c2eef-6c23-8f96-9391-d8bb537c557d","beb01ab0-4596-11e3-8f96-0800200c9a66"}, //Sunhold Medic
            {"fa084467-da67-7580-2683-36c018ec6fc1","aab8bda3-f7fc-11e3-a3ac-0800200c9a66"}, //Suppress
            {"8fdb52e8-d34e-5f52-73e9-881448c79cc4","04bffff1-673b-11e3-949a-0800200c9a66"}, //Supreme Atromancer
            {"5c472390-f28f-74ba-bdd2-0218962f260f","00f37248-52f0-11e3-8f96-0800200c9a66"}, //Swamp Leviathan
            {"a711c156-1038-2e6d-a440-3c80a0d26f10","8e0d6f71-f400-11e4-b939-0800200c9a66"}, //Swift Strike
            {"28bc7476-4193-fe91-7567-365dfa4b4006","e62d3cc4-6650-11e6-bdf4-0800200c9a66"}, //Swindler's Market
            {"ed038dfb-a539-0df1-b91a-56e857c2ec23","beaff3aa-4596-11e3-8f96-0800200c9a66"}, //Tazkad the Packmaster
            {"9d909ebc-88d3-8020-dfaf-38a73184dd7e","1a0d2aa0-16cf-11e5-b939-0800200c9a66"}, //Telvanni Arcanist
            {"2964d194-628d-54c8-b6e4-ebedf79b3e96","fd235b52-0afb-11e5-b939-0800200c9a66"}, //Tenmar Swiftclaw
            {"118c9075-790c-a109-a632-8e38234636d4","5e26d8d8-f451-11e4-b939-0800200c9a66"}, //Territorial Viper
            {"4b692a01-283c-8aa6-b655-5e6dff094294","d0d4130f-a8a0-11e3-a5e2-0800200c9a66"}, //Thievery
            {"738e5861-9a27-9275-7d4a-9b521f697159","fd235b5a-0afb-11e5-b939-0800200c9a66"}, //Thieves' Den
            {"9699f662-6163-23c8-28b0-9f1c50904854","fd235b60-0afb-11e5-b939-0800200c9a66"}, //Thieves Guild Recruit
            {"1c47b056-8fea-1987-f84a-7feeb75aed8f","d0d4130b-a8a0-11e3-a5e2-0800200c9a66"}, //Thorn Histmage
            {"6a61eab1-48a1-a988-ffa3-04aa88cdb461","5e26d8d6-f451-11e4-b939-0800200c9a66"}, //Tome of Alteration
            {"f7be8749-b993-1278-312d-071c6d03f148","fd235b53-0afb-11e5-b939-0800200c9a66"}, //Torval Crook
            {"84b0450e-4018-60fc-e30f-0a3c195233bd","0cd2564d-e227-11e3-8b68-0800200c9a66"}, //Tower Alchemist
            {"c186839e-0068-970a-3d89-adaf0b3322bc","33acb181-300a-11e5-a2cb-0800200c9a66"}, //Trebuchet
            {"09587125-e857-84e4-762f-12457f6b864f","beb041c4-4596-11e3-8f96-0800200c9a66"}, //Tree Minder
            {"29067ad5-b7db-56f6-4ceb-2a5544b39dc9","33acb173-300a-11e5-a2cb-0800200c9a66"}, //Triumphant Jarl
            {"2c46ac22-d826-9424-c921-ad416075e116","3d4251c1-7983-11e3-981f-0800200c9a66"}, //Tusked Bristleback
            {"fc7d307b-280a-a4d5-da6e-7e860785d005","33acb177-300a-11e5-a2cb-0800200c9a66"}, //Twilight Werebat
            {"04e6e6fd-5ce1-fd7e-a3ab-e39bbe385eee","08662205-ff5d-11e4-b939-0800200c9a66"}, //Two-Moons Contemplation
            {"2a26756d-0aae-c321-efab-696c125232d0","1959e280-ff5d-11e4-b939-0800200c9a66"}, //Tyr
            {"bed7199f-d5a0-3edd-bbef-ac7fbc552c2b","fd233440-0afb-11e5-b939-0800200c9a66"}, //Ungolim the Listener
            {"1d723109-00b7-8371-a3ac-5e250e8848ed","377112f2-dfb5-46b9-b24b-a104c085e0f3"}, //Valenwood Huntsman
            {"2ca21d29-023b-6add-83f5-99223b429b20","e967a7f0-a458-11e5-a837-0800200c9a66"}, //Valenwood Trapper
            {"d235a8e4-9bde-fbc2-b582-c105b04a7d85","0cd25650-e227-11e3-8b68-0800200c9a66"}, //Varanis Courier
            {"6ac62a44-0e18-70a0-1246-4e8a08058ebb","beb01ab3-4596-11e3-8f96-0800200c9a66"}, //Vicious Dreugh
            {"aeb3b722-267c-5cfb-3a90-a11deeabcd82","631142f1-300a-11e5-a2cb-0800200c9a66"}, //Vigilant Giant
            {"03ccf0ab-56d9-533e-90ed-2936f8216481","90431f47-94e0-11e3-baa8-0800200c9a66"}, //Volendrung
            {"06c3c7ef-61c6-cf10-cf62-2b7ad6005916","7e6861fa-20ce-11e5-867f-0800200c9a66"}, //Volkihar Lord
            {"e5fe117f-5eba-ecd8-0008-7ee17d3d5815","fbcc37e4-78b7-11e3-981f-0800200c9a66"}, //Voracious Spriggan
            {"bb9e48d0-c7a7-f2a4-c2d1-0fa1e292c0a7","90431f49-94e0-11e3-baa8-0800200c9a66"}, //Wabbajack
            {"ea0d3482-8198-6c27-bd61-1cd3c035aa5d","90431f40-94e0-11e3-baa8-0800200c9a66"}, //War Cry
            {"5ae3fa65-90f5-0587-4f36-764b5becdf22","69e8b671-87a1-11e3-baa7-0800200c9a66"}, //Wardcrafter
            {"29e1fc26-f783-36f2-f24c-86d06ea80295","0e06fa5a-1112-11e5-b939-0800200c9a66"}, //Watch Commander
            {"b27dde66-7e0b-21e0-5efe-519faa87c5a2","441521f2-300a-11e5-a2cb-0800200c9a66"}, //Whirling Duelist
            {"fd37c6c4-42da-238b-bde9-696fee288534","d0d41312-a8a0-11e3-a5e2-0800200c9a66"}, //Whiterun Recruit
            {"4ab4bd53-ce78-a82e-6c69-4c65194900df","beadd0c6-4596-11e3-8f96-0800200c9a66"}, //Whiterun Trooper
            {"7860f3de-262c-b5e2-3d09-b7cb1dc80717","fd235b5b-0afb-11e5-b939-0800200c9a66"}, //Wild Beastcaller
            {"53f48fda-59ab-8bed-e3ce-1a4aaea92e93","7e6861f7-20ce-11e5-867f-0800200c9a66"}, //Wild Spriggan
            {"583b0bca-af97-6b4d-2cd7-57ea1a6dc47f","00f37253-52f0-11e3-8f96-0800200c9a66"}, //Wind Keep Spellsword
            {"de42a506-dd51-adb3-e2b2-078ca5467f42","3a751804-e810-11e3-ac10-0800200c9a66"}, //Winter's Grasp
            {"f06ebf8e-380c-6ff7-f838-b8e29a1fd6c5","e967a7f3-a458-11e5-a837-0800200c9a66"}, //Winter's Touch
            {"9eb66227-f1d9-ec29-a31a-46644c5fd4d2","69e8b672-87a1-11e3-baa7-0800200c9a66"}, //Wisdom of Ancients
            {"77b05e01-e47a-ec90-fa7b-be9775bfb75d","3a751800-e810-11e3-ac10-0800200c9a66"}, //Wispmother
            {"48abb215-403f-4d40-438a-8ade2624755b","aab8e4b9-f7fc-11e3-a3ac-0800200c9a66"}, //Withered Hand Cultist
            {"ffcc4636-a48d-9044-f659-8fd0f6f029e6","fbcc5ef0-78b7-11e3-981f-0800200c9a66"}, //Wood Orc Headhunter
            {"051cda36-f0ac-8606-0c05-ef9f3e606ca6","fbcc37f2-78b7-11e3-981f-0800200c9a66"}, //Wrothgar Artisan
            {"bfce2497-8d14-e00f-de61-b64aa4907de4","90431f3f-94e0-11e3-baa8-0800200c9a66"}, //Wrothgar Forge
            {"2482cc73-a5e0-b01c-e502-75176ea0e9b3","beaff3a2-4596-11e3-8f96-0800200c9a66"}, //Wrothgar Kingpin
            {"6b1f405d-cd9b-7e61-06c2-e3422c6aeb00","6e153751-f451-11e4-b939-0800200c9a66"}, //Yew Shield
            {"f95f6151-4064-85cf-c723-61c2a687bbb6","beaff3ac-4596-11e3-8f96-0800200c9a66"}, //Young Mammoth


        };




        IEnumerable<Card> cards;
        public IEnumerable<Card> Cards
        {
            get
            {
                return cards;
            }
            set
            {
                cards = value;
            }
        }

        public Version Version { get; set; }
        public string VersionInfo { get; set; }
        public DateTime VersionDate { get; set; }

        IEnumerable<CardSet> cardSets;
        public IEnumerable<CardSet> CardSets
        {
            get
            {
                return cardSets;
            }
            set
            {
                cardSets = value;
            }
        }

        private CardsDatabase()
        {

        }

        public static CardsDatabase LoadCardsDatabase(string datbasePath)
        {
            if (new Utils.IOWrappers.FileWrapper().Exists(datbasePath))
            {
                CardsDatabase database = SerializationHelper.DeserializeJson<CardsDatabase>(System.IO.File.ReadAllText(datbasePath));
                return database;
            }
            else
            {
                return null;
            }
        }

        public Card FindCardByName(string name)
        {
            return Cards.Where(c => c.Name.ToLower().Trim() == name.ToLower().Trim()).DefaultIfEmpty(Card.Unknown).FirstOrDefault();
        }

        public Card FindCardById(Guid value)
        {
            var ret = Cards.Where(c => c.Id == value).DefaultIfEmpty(Card.Unknown).FirstOrDefault();
            if (ret == Card.Unknown)
            {
                if (GuidTranslation.ContainsKey(value.ToString().ToLower()))
                {
                    ret = Cards.Where(c => c.Id == Guid.Parse(GuidTranslation[value.ToString().ToLower()])).DefaultIfEmpty(Card.Unknown).FirstOrDefault();
                }
                if ((ret == Card.Unknown) && GuidTranslationCore.ContainsKey(value.ToString().ToLower()))
                {
                    ret = Cards.Where(c => c.Id == Guid.Parse(GuidTranslationCore[value.ToString().ToLower()])).DefaultIfEmpty(Card.Unknown).FirstOrDefault();
                }
            }
            return ret;
        }



        public CardSet FindCardSetById(Guid? value)
        {
            if (value.HasValue)
            {
                return CardSets.Where(c => c.Id == value).SingleOrDefault();
            }
            else
            {
                return null;
            }
        }

        public CardSet FindCardSetByName(string value)
        {
            return CardSets.Where(c => c.Name == value).SingleOrDefault();
        }

        public IEnumerable<string> GetCardsNames(string setFilter = null)
        {
            return Cards.Where(c => String.IsNullOrEmpty(setFilter) || (c.Set == setFilter)).Select(c => c.Name);
        }
    }
}
