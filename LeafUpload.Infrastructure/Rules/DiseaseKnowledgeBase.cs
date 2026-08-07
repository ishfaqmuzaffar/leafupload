using LeafUpload.Core.Models;
using System;
using System.Collections.Generic;

namespace LeafUpload.Infrastructure.Rules
{
    // Keyed by the model's exact raw label strings (e.g. "Apple___Apple_scab") -
    // see LeafUpload.Infrastructure/MLModel1.mlnet's training labels. Only the
    // 26 actual-disease labels need entries; every "___healthy" label is
    // handled generically by Lookup() below rather than duplicated per crop.
    public static class DiseaseKnowledgeBase
    {
        public static DiseaseInfo? Lookup(string rawLabel, string cultureCode)
        {
            if (string.IsNullOrWhiteSpace(rawLabel))
                return null;

            if (rawLabel.Contains("healthy", StringComparison.OrdinalIgnoreCase))
                return cultureCode switch { "hi" => HealthyHi, "ur" => HealthyUr, _ => HealthyEn };

            var dict = ByCulture(cultureCode);
            if (dict.TryGetValue(rawLabel, out var info))
                return info;

            // Missing translation for this label - fall back to English rather
            // than showing nothing.
            return English.TryGetValue(rawLabel, out var en) ? en : null;
        }

        private static Dictionary<string, DiseaseInfo> ByCulture(string cultureCode) => cultureCode switch
        {
            "hi" => Hindi,
            "ur" => Urdu,
            _ => English,
        };

        private static readonly DiseaseInfo HealthyEn = new()
        {
            Symptoms = Array.Empty<string>(),
            Treatment = "No disease detected. Keep monitoring your crop and maintain good watering and airflow.",
        };
        private static readonly DiseaseInfo HealthyHi = new()
        {
            Symptoms = Array.Empty<string>(),
            Treatment = "कोई रोग नहीं पाया गया। अपनी फसल की निगरानी जारी रखें और सिंचाई व हवा के आवागमन का ध्यान रखें।",
        };
        private static readonly DiseaseInfo HealthyUr = new()
        {
            Symptoms = Array.Empty<string>(),
            Treatment = "کوئی بیماری نہیں پائی گئی۔ اپنی فصل کی نگرانی جاری رکھیں اور آبپاشی اور ہوا کی آمدورفت کا خیال رکھیں۔",
        };

        private static readonly Dictionary<string, DiseaseInfo> English = new()
        {
            ["Apple___Apple_scab"] = new DiseaseInfo
            {
                Symptoms = new[]
                {
                    "Olive-green to brown velvety spots on leaves that later turn scabby or corky",
                    "Similar dark, rough patches on the fruit skin",
                    "Heavily infected leaves turn yellow and drop early",
                },
                Treatment = "Remove and destroy fallen leaves to reduce next season's spores, and apply a fungicide labeled for apple scab starting at bud break, especially in wet spring weather.",
            },
            ["Apple___Black_rot"] = new DiseaseInfo
            {
                Symptoms = new[]
                {
                    "Purple-bordered brown spots on leaves, sometimes called \"frog-eye\" spots",
                    "Sunken, black-bordered lesions on fruit that rot from the inside",
                    "Reddish-brown cankers on branches",
                },
                Treatment = "Prune out and destroy cankered wood and mummified fruit, apply a fungicide during the growing season, and remove nearby dead wood that harbors the fungus.",
            },
            ["Apple___Cedar_apple_rust"] = new DiseaseInfo
            {
                Symptoms = new[]
                {
                    "Bright yellow-orange spots on the upper leaf surface that enlarge over summer",
                    "Small black dots appearing within the spots",
                    "Tube-like fungal structures on the underside of leaves in humid weather",
                },
                Treatment = "Remove nearby juniper or cedar trees if possible (they host the other half of this fungus's life cycle), and apply a protective fungicide from pink bud stage through early summer.",
            },
            ["Cherry_(including_sour)___Powdery_mildew"] = new DiseaseInfo
            {
                Symptoms = new[]
                {
                    "White powdery patches on leaves and shoots, often starting on new growth",
                    "Leaves curling, puckering, or turning pale",
                    "Stunted shoot growth in severe cases",
                },
                Treatment = "Improve airflow with pruning, avoid excess nitrogen fertilizer, and apply a sulfur-based or other labeled fungicide at the first sign of white patches.",
            },
            ["Corn_(maize)___Cercospora_leaf_spot Gray_leaf_spot"] = new DiseaseInfo
            {
                Symptoms = new[]
                {
                    "Small tan to gray rectangular lesions running parallel to leaf veins",
                    "Lesions expanding and merging into gray-brown patches",
                    "Symptoms worsen in humid, warm weather with heavy crop residue nearby",
                },
                Treatment = "Rotate away from corn for a season, till under old crop residue where the fungus survives, and apply a foliar fungicide if disease appears before tasseling in susceptible fields.",
            },
            ["Corn_(maize)___Common_rust_"] = new DiseaseInfo
            {
                Symptoms = new[]
                {
                    "Small, raised, cinnamon-brown pustules scattered on both leaf surfaces",
                    "Pustules darkening toward black late in the season",
                    "Heavy infection can yellow and dry out leaves",
                },
                Treatment = "Plant rust-resistant hybrids where available - a fungicide is usually only needed if rust appears early and heavily on young plants.",
            },
            ["Corn_(maize)___Northern_Leaf_Blight"] = new DiseaseInfo
            {
                Symptoms = new[]
                {
                    "Long, cigar-shaped gray-green to tan lesions running parallel to the leaf",
                    "Symptoms often start on lower, older leaves first",
                    "Lesions merging and killing large sections of leaf in heavy infections",
                },
                Treatment = "Rotate crops and till residue to reduce carryover, choose resistant hybrids, and apply a fungicide if lesions appear on upper leaves before or during tasseling.",
            },
            ["Grape___Black_rot"] = new DiseaseInfo
            {
                Symptoms = new[]
                {
                    "Small tan spots with dark borders on leaves",
                    "Black, shriveled \"mummy\" berries on the cluster",
                    "Tiny black fungal dots visible on infected fruit and leaf spots",
                },
                Treatment = "Remove mummified berries and infected canes during pruning, improve canopy airflow, and apply a fungicide program from bud break through fruit set in wet seasons.",
            },
            ["Grape___Esca_(Black_Measles)"] = new DiseaseInfo
            {
                Symptoms = new[]
                {
                    "\"Tiger-stripe\" pattern of yellow or red stripes between leaf veins",
                    "Dark spots on berries in severe cases",
                    "Sudden wilting and dieback of shoots in advanced infections",
                },
                Treatment = "There is no cure once the wood is infected - prune out and destroy affected wood well below the visible symptoms, avoid pruning in wet weather, and protect pruning cuts.",
            },
            ["Grape___Leaf_blight_(Isariopsis_Leaf_Spot)"] = new DiseaseInfo
            {
                Symptoms = new[]
                {
                    "Angular reddish-brown spots on leaves",
                    "Spots merging into larger dead patches",
                    "Heavily spotted leaves yellowing and dropping early, weakening the vine",
                },
                Treatment = "Improve canopy airflow through pruning, remove fallen infected leaves, and apply a labeled fungicide during humid periods.",
            },
            ["Orange___Haunglongbing_(Citrus_greening)"] = new DiseaseInfo
            {
                Symptoms = new[]
                {
                    "Blotchy yellow mottling on leaves that does not mirror across the leaf's midrib",
                    "Stunted, lopsided, bitter-tasting fruit",
                    "Twig dieback and a thinning canopy over time",
                },
                Treatment = "There is no cure - remove and destroy confirmed infected trees to slow spread, and control the Asian citrus psyllid insect that spreads the disease with a recommended insecticide programme.",
            },
            ["Peach___Bacterial_spot"] = new DiseaseInfo
            {
                Symptoms = new[]
                {
                    "Small water-soaked spots on leaves that turn purple-black and often fall out, leaving a \"shot-hole\" look",
                    "Dark, sunken spots on fruit",
                    "Early leaf drop in heavy infections",
                },
                Treatment = "Prune for airflow, avoid overhead irrigation that splashes bacteria between leaves, and apply a copper-based bactericide during the dormant season and early growing season.",
            },
            ["Pepper,_bell___Bacterial_spot"] = new DiseaseInfo
            {
                Symptoms = new[]
                {
                    "Small dark, water-soaked spots on leaves with yellow halos",
                    "Spots merging and causing leaves to drop",
                    "Raised, scabby spots on fruit",
                },
                Treatment = "Use disease-free seed or transplants, avoid working in wet fields, rotate crops, and apply a copper-based bactericide at the first sign of spotting.",
            },
            ["Potato___Early_blight"] = new DiseaseInfo
            {
                Symptoms = new[]
                {
                    "Dark brown spots with concentric rings (a \"target\" pattern), usually on older, lower leaves first",
                    "Yellowing of leaf tissue around the spots",
                    "Can also cause dark, sunken lesions on tubers",
                },
                Treatment = "Rotate crops away from potato and tomato, remove infected foliage, keep plants well-fed (stressed plants are more susceptible), and apply a fungicide if spots appear early in the season.",
            },
            ["Potato___Late_blight"] = new DiseaseInfo
            {
                Symptoms = new[]
                {
                    "Water-soaked pale to dark green blotches on leaves that turn brown or black and spread fast",
                    "White fungal fuzz on the underside of leaves in humid weather",
                    "Can rot an entire plant within days",
                },
                Treatment = "This spreads very fast - remove and destroy infected plants immediately, avoid overhead watering, and apply a protective fungicide proactively in cool, wet weather.",
            },
            ["Squash___Powdery_mildew"] = new DiseaseInfo
            {
                Symptoms = new[]
                {
                    "White powdery spots on upper and lower leaf surfaces and stems",
                    "Symptoms usually starting on older leaves first",
                    "Leaves yellowing, curling, and dying back as mildew spreads",
                },
                Treatment = "Plant in full sun with good spacing for airflow, remove heavily infected leaves, and apply a sulfur-based or other labeled fungicide at first appearance.",
            },
            ["Strawberry___Leaf_scorch"] = new DiseaseInfo
            {
                Symptoms = new[]
                {
                    "Small dark purple spots on leaves that enlarge and merge",
                    "Leaves taking on a scorched, reddish-brown appearance",
                    "Reduced plant vigor and yield in severe infections",
                },
                Treatment = "Remove old and infected leaves after harvest, avoid overhead watering, ensure good spacing for airflow, and apply a labeled fungicide if the disease is recurring.",
            },
            ["Tomato___Bacterial_spot"] = new DiseaseInfo
            {
                Symptoms = new[]
                {
                    "Small dark, greasy-looking spots on leaves, often with a yellow halo",
                    "Raised, scabby lesions on fruit",
                    "Heavy infection causing leaf drop",
                },
                Treatment = "Use disease-free seed or transplants, avoid overhead watering and working with plants when wet, rotate crops, and apply a copper-based bactericide at the first sign.",
            },
            ["Tomato___Early_blight"] = new DiseaseInfo
            {
                Symptoms = new[]
                {
                    "Dark brown spots with concentric \"target\" rings, usually on older, lower leaves first",
                    "Yellowing of leaf tissue around the spots",
                    "Can also cause sunken dark lesions near the stem end of fruit",
                },
                Treatment = "Remove lower infected leaves, mulch to stop soil from splashing onto leaves, rotate crops, and apply a fungicide if spotting spreads up the plant.",
            },
            ["Tomato___Late_blight"] = new DiseaseInfo
            {
                Symptoms = new[]
                {
                    "Large, water-soaked greenish-black blotches on leaves that spread rapidly",
                    "White fungal growth on the underside of leaves in humid conditions",
                    "Brown, firm-to-greasy rot on fruit",
                },
                Treatment = "This spreads fast and can wipe out a crop within days - remove and destroy infected plants immediately, avoid overhead watering, and apply a protective fungicide proactively in cool, wet weather.",
            },
            ["Tomato___Leaf_Mold"] = new DiseaseInfo
            {
                Symptoms = new[]
                {
                    "Pale yellow spots on the upper leaf surface",
                    "Olive-green to grayish-purple velvety mold on the underside, directly below the yellow spots",
                    "Worse in humid, poorly ventilated greenhouses or tunnels",
                },
                Treatment = "Improve airflow and reduce humidity around plants (space plants out, prune lower leaves, ventilate greenhouses), and apply a labeled fungicide if the problem persists.",
            },
            ["Tomato___Septoria_leaf_spot"] = new DiseaseInfo
            {
                Symptoms = new[]
                {
                    "Many small circular spots with dark borders and tan or gray centers, usually starting on lower leaves",
                    "Tiny black specks visible in the centers of spots",
                    "Heavy spotting causing leaves to yellow and drop",
                },
                Treatment = "Remove infected lower leaves, avoid overhead watering, mulch to reduce soil splash, rotate crops, and apply a fungicide if spotting is spreading.",
            },
            ["Tomato___Spider_mites Two-spotted_spider_mite"] = new DiseaseInfo
            {
                Symptoms = new[]
                {
                    "Fine yellow or white stippling/speckling on leaves",
                    "Fine webbing on the underside of leaves and between stems in heavy infestations",
                    "Leaves bronzing, drying out, and dropping",
                },
                Treatment = "Rinse plants with a strong water spray to knock mites off, encourage natural predators, and use an insecticidal soap or miticide if the infestation is heavy - fungicides won't treat this, since it's caused by mites, not a fungus.",
            },
            ["Tomato___Target_Spot"] = new DiseaseInfo
            {
                Symptoms = new[]
                {
                    "Brown spots with concentric target-like rings on leaves, stems, and fruit",
                    "Spots merging and causing significant leaf yellowing and drop",
                    "Worse in humid conditions",
                },
                Treatment = "Improve airflow through pruning and spacing, remove infected leaves and debris, rotate crops, and apply a labeled fungicide if disease is spreading.",
            },
            ["Tomato___Tomato_mosaic_virus"] = new DiseaseInfo
            {
                Symptoms = new[]
                {
                    "Light and dark green mottled, mosaic-like pattern on leaves",
                    "Leaves that are curled, narrow, or fern-like",
                    "Stunted plant growth and reduced, sometimes mottled, fruit",
                },
                Treatment = "There is no cure - remove and destroy infected plants to stop spread, wash hands and tools between plants (the virus spreads easily by touch), and avoid tobacco use near plants, since the virus can survive in tobacco products.",
            },
            ["Tomato___Tomato_Yellow_Leaf_Curl_Virus"] = new DiseaseInfo
            {
                Symptoms = new[]
                {
                    "Leaves curling sharply upward and turning yellow, especially at the edges",
                    "Visibly stunted plants with shortened stems",
                    "Flowers dropping before setting fruit",
                },
                Treatment = "There is no cure - remove and destroy infected plants promptly, and control whiteflies (which spread this virus) with sticky traps, reflective mulch, or a labeled insecticide, since managing the insect is the main way to prevent spread.",
            },
        };

        private static readonly Dictionary<string, DiseaseInfo> Hindi = new()
        {
            ["Apple___Apple_scab"] = new DiseaseInfo
            {
                Symptoms = new[]
                {
                    "पत्तियों पर जैतूनी-हरे से भूरे रंग के मखमली धब्बे जो बाद में खुरदरे हो जाते हैं",
                    "फल की त्वचा पर भी ऐसे ही गहरे, खुरदरे धब्बे",
                    "अधिक संक्रमित पत्तियाँ पीली होकर जल्दी गिर जाती हैं",
                },
                Treatment = "अगले सीज़न के बीजाणु कम करने के लिए गिरी हुई पत्तियों को हटाकर नष्ट करें, और नमी भरे वसंत मौसम में कली फूटने से ही एप्पल स्कैब के लिए अनुमोदित फफूंदनाशक का छिड़काव करें।",
            },
            ["Apple___Black_rot"] = new DiseaseInfo
            {
                Symptoms = new[]
                {
                    "पत्तियों पर बैंगनी किनारे वाले भूरे धब्बे (\"मेंढक-आँख\" जैसे धब्बे)",
                    "फल पर धँसे हुए, काले किनारे वाले घाव जो अंदर से सड़ते हैं",
                    "शाखाओं पर लाल-भूरे रंग के कैंकर",
                },
                Treatment = "छँटाई के दौरान कैंकर-युक्त लकड़ी और सूखे हुए फलों को हटाकर नष्ट करें, बढ़ते मौसम में फफूंदनाशक लगाएं, और आसपास की मृत लकड़ी हटा दें जो फफूंद को आश्रय देती है।",
            },
            ["Apple___Cedar_apple_rust"] = new DiseaseInfo
            {
                Symptoms = new[]
                {
                    "पत्ती की ऊपरी सतह पर चमकीले पीले-नारंगी धब्बे जो गर्मियों में बड़े होते जाते हैं",
                    "धब्बों के भीतर छोटे काले बिंदु दिखाई देना",
                    "नम मौसम में पत्ती के नीचे की ओर ट्यूब जैसी संरचनाएँ",
                },
                Treatment = "यदि संभव हो तो पास के देवदार/जुनिपर पेड़ों को हटा दें (ये इस फफूंद के जीवन-चक्र का दूसरा हिस्सा हैं), और गुलाबी कली अवस्था से लेकर शुरुआती गर्मियों तक सुरक्षात्मक फफूंदनाशक लगाएं।",
            },
            ["Cherry_(including_sour)___Powdery_mildew"] = new DiseaseInfo
            {
                Symptoms = new[]
                {
                    "पत्तियों और टहनियों पर सफ़ेद पाउडर जैसे धब्बे, जो अक्सर नई वृद्धि पर शुरू होते हैं",
                    "पत्तियों का मुड़ना, सिकुड़ना, या पीला पड़ना",
                    "गंभीर मामलों में टहनी की वृद्धि रुक जाना",
                },
                Treatment = "छँटाई से हवा का आवागमन बेहतर करें, अधिक नाइट्रोजन उर्वरक से बचें, और सफ़ेद धब्बे दिखते ही सल्फर-आधारित या अन्य अनुमोदित फफूंदनाशक लगाएं।",
            },
            ["Corn_(maize)___Cercospora_leaf_spot Gray_leaf_spot"] = new DiseaseInfo
            {
                Symptoms = new[]
                {
                    "पत्ती की शिराओं के समानांतर छोटे तन-भूरे से धूसर आयताकार धब्बे",
                    "धब्बे बढ़कर आपस में मिलकर धूसर-भूरे पैच बन जाना",
                    "नम, गर्म मौसम और अधिक फ़सल अवशेष होने पर लक्षण बदतर होना",
                },
                Treatment = "एक सीज़न के लिए मक्का न लगाएं, पुराने फ़सल अवशेष को मिट्टी में मिला दें जहाँ फफूंद जीवित रहती है, और यदि संवेदनशील खेतों में टेसलिंग से पहले रोग दिखे तो पर्णीय फफूंदनाशक लगाएं।",
            },
            ["Corn_(maize)___Common_rust_"] = new DiseaseInfo
            {
                Symptoms = new[]
                {
                    "पत्ती की दोनों सतहों पर छोटे, उभरे हुए, दालचीनी-भूरे रंग के फुंसी जैसे धब्बे",
                    "सीज़न के अंत तक धब्बे काले पड़ जाना",
                    "अधिक संक्रमण से पत्तियाँ पीली होकर सूख सकती हैं",
                },
                Treatment = "जहाँ उपलब्ध हो वहाँ रस्ट-प्रतिरोधी किस्में लगाएं - फफूंदनाशक की आवश्यकता आमतौर पर तभी होती है जब यह रोग युवा पौधों पर जल्दी और भारी मात्रा में दिखे।",
            },
            ["Corn_(maize)___Northern_Leaf_Blight"] = new DiseaseInfo
            {
                Symptoms = new[]
                {
                    "पत्ती के समानांतर लंबे, सिगार के आकार के धूसर-हरे से तन रंग के घाव",
                    "लक्षण अक्सर नीचे की पुरानी पत्तियों से शुरू होते हैं",
                    "गंभीर संक्रमण में घाव आपस में मिलकर पत्ती के बड़े हिस्से को नष्ट कर देते हैं",
                },
                Treatment = "फ़सल चक्र अपनाएं और अवशेष को मिट्टी में मिलाएं, प्रतिरोधी किस्में चुनें, और यदि टेसलिंग से पहले या उसके दौरान ऊपरी पत्तियों पर घाव दिखें तो फफूंदनाशक लगाएं।",
            },
            ["Grape___Black_rot"] = new DiseaseInfo
            {
                Symptoms = new[]
                {
                    "पत्तियों पर गहरे किनारे वाले छोटे तन रंग के धब्बे",
                    "गुच्छे पर काले, सिकुड़े हुए \"ममी\" जामुन",
                    "संक्रमित फल और पत्ती के धब्बों पर छोटे काले फफूंद बिंदु दिखना",
                },
                Treatment = "छँटाई के दौरान ममी बने जामुन और संक्रमित बेलों को हटा दें, छतरी में हवा का आवागमन बेहतर करें, और नम मौसम में कली फूटने से लेकर फल बनने तक फफूंदनाशक कार्यक्रम अपनाएं।",
            },
            ["Grape___Esca_(Black_Measles)"] = new DiseaseInfo
            {
                Symptoms = new[]
                {
                    "पत्ती की शिराओं के बीच पीली/लाल धारियों का \"बाघ-धारी\" पैटर्न",
                    "गंभीर मामलों में जामुन पर गहरे धब्बे",
                    "उन्नत संक्रमण में टहनियों का अचानक मुरझाना और सूखना",
                },
                Treatment = "एक बार लकड़ी संक्रमित होने पर कोई इलाज नहीं है - दिखाई देने वाले लक्षणों से काफी नीचे तक संक्रमित लकड़ी को काटकर नष्ट करें, नम मौसम में छँटाई से बचें, और छँटाई के घावों को सुरक्षित रखें।",
            },
            ["Grape___Leaf_blight_(Isariopsis_Leaf_Spot)"] = new DiseaseInfo
            {
                Symptoms = new[]
                {
                    "पत्तियों पर कोणीय लाल-भूरे धब्बे",
                    "धब्बे मिलकर बड़े मृत क्षेत्र बनाना",
                    "अधिक धब्बों वाली पत्तियाँ पीली होकर जल्दी गिरना, जिससे बेल कमज़ोर होती है",
                },
                Treatment = "छँटाई से छतरी में हवा का आवागमन बेहतर करें, गिरी हुई संक्रमित पत्तियों को हटाएं, और नम अवधि में अनुमोदित फफूंदनाशक लगाएं।",
            },
            ["Orange___Haunglongbing_(Citrus_greening)"] = new DiseaseInfo
            {
                Symptoms = new[]
                {
                    "पत्तियों पर धब्बेदार पीलापन जो मध्य शिरा के आर-पार सममित नहीं होता",
                    "छोटे, असमान आकार के, कड़वे स्वाद वाले फल",
                    "समय के साथ टहनियों का सूखना और छतरी का पतला होना",
                },
                Treatment = "इसका कोई इलाज नहीं है - फैलाव रोकने के लिए पुष्ट संक्रमित पेड़ों को हटाकर नष्ट करें, और इस रोग को फैलाने वाले एशियाई सिट्रस साइलिड कीट को अनुशंसित कीटनाशक कार्यक्रम से नियंत्रित करें।",
            },
            ["Peach___Bacterial_spot"] = new DiseaseInfo
            {
                Symptoms = new[]
                {
                    "पत्तियों पर छोटे पानी-भीगे धब्बे जो बैंगनी-काले होकर अक्सर गिर जाते हैं, जिससे \"छर्रे के छेद\" जैसा रूप बनता है",
                    "फल पर गहरे, धँसे हुए धब्बे",
                    "गंभीर संक्रमण में जल्दी पत्ती गिरना",
                },
                Treatment = "हवा के आवागमन हेतु छँटाई करें, ओवरहेड सिंचाई से बचें जो पत्तियों के बीच जीवाणु फैलाती है, और सुप्त मौसम व बढ़ते मौसम की शुरुआत में कॉपर-आधारित बैक्टीरियानाशक लगाएं।",
            },
            ["Pepper,_bell___Bacterial_spot"] = new DiseaseInfo
            {
                Symptoms = new[]
                {
                    "पीले घेरे वाले छोटे गहरे, पानी-भीगे धब्बे पत्तियों पर",
                    "धब्बे मिलकर पत्तियों के गिरने का कारण बनना",
                    "फल पर उभरे, खुरदरे धब्बे",
                },
                Treatment = "रोग-मुक्त बीज/पौध का उपयोग करें, गीले खेत में काम करने से बचें, फ़सल चक्र अपनाएं, और धब्बे दिखते ही कॉपर-आधारित बैक्टीरियानाशक लगाएं।",
            },
            ["Potato___Early_blight"] = new DiseaseInfo
            {
                Symptoms = new[]
                {
                    "गोलाकार छल्लों वाले गहरे भूरे धब्बे (\"निशाना\" पैटर्न), पहले पुरानी, निचली पत्तियों पर",
                    "धब्बों के आसपास पत्ती के ऊतक का पीला पड़ना",
                    "कंद पर गहरे, धँसे हुए घाव भी हो सकते हैं",
                },
                Treatment = "आलू और टमाटर से फ़सल चक्र बदलें, संक्रमित पत्तियाँ हटाएं, पौधों को अच्छी तरह पोषित रखें (तनावग्रस्त पौधे अधिक संवेदनशील होते हैं), और यदि सीज़न की शुरुआत में धब्बे दिखें तो फफूंदनाशक लगाएं।",
            },
            ["Potato___Late_blight"] = new DiseaseInfo
            {
                Symptoms = new[]
                {
                    "पत्तियों पर पानी-भीगे हल्के से गहरे हरे धब्बे जो भूरे/काले होकर तेज़ी से फैलते हैं",
                    "नम मौसम में पत्ती के नीचे सफ़ेद फफूंदी परत",
                    "कुछ ही दिनों में पूरा पौधा सड़ सकता है",
                },
                Treatment = "यह बहुत तेज़ी से फैलता है - संक्रमित पौधों को तुरंत हटाकर नष्ट करें, ओवरहेड सिंचाई से बचें, और ठंडे, नम मौसम में पहले से ही सुरक्षात्मक फफूंदनाशक लगाएं।",
            },
            ["Squash___Powdery_mildew"] = new DiseaseInfo
            {
                Symptoms = new[]
                {
                    "पत्ती की ऊपरी और निचली सतहों तथा तनों पर सफ़ेद पाउडर जैसे धब्बे",
                    "लक्षण आमतौर पर पुरानी पत्तियों पर पहले शुरू होते हैं",
                    "फफूंदी फैलने के साथ पत्तियाँ पीली होकर मुड़ती और सूखती हैं",
                },
                Treatment = "हवा के आवागमन के लिए पूर्ण धूप और उचित दूरी पर रोपें, अधिक संक्रमित पत्तियाँ हटाएं, और लक्षण दिखते ही सल्फर-आधारित या अन्य अनुमोदित फफूंदनाशक लगाएं।",
            },
            ["Strawberry___Leaf_scorch"] = new DiseaseInfo
            {
                Symptoms = new[]
                {
                    "पत्तियों पर छोटे गहरे बैंगनी धब्बे जो बढ़कर आपस में मिल जाते हैं",
                    "पत्तियाँ झुलसी हुई, लाल-भूरी दिखने लगती हैं",
                    "गंभीर संक्रमण में पौधे की शक्ति और उपज कम होना",
                },
                Treatment = "कटाई के बाद पुरानी और संक्रमित पत्तियाँ हटाएं, ओवरहेड सिंचाई से बचें, हवा के आवागमन हेतु उचित दूरी रखें, और यदि रोग बार-बार हो तो अनुमोदित फफूंदनाशक लगाएं।",
            },
            ["Tomato___Bacterial_spot"] = new DiseaseInfo
            {
                Symptoms = new[]
                {
                    "पत्तियों पर छोटे गहरे, चिकने दिखने वाले धब्बे, अक्सर पीले घेरे के साथ",
                    "फल पर उभरे, खुरदरे घाव",
                    "अधिक संक्रमण से पत्तियाँ गिरना",
                },
                Treatment = "रोग-मुक्त बीज/पौध का उपयोग करें, ओवरहेड सिंचाई और गीले पौधों के साथ काम करने से बचें, फ़सल चक्र अपनाएं, और लक्षण दिखते ही कॉपर-आधारित बैक्टीरियानाशक लगाएं।",
            },
            ["Tomato___Early_blight"] = new DiseaseInfo
            {
                Symptoms = new[]
                {
                    "गोलाकार छल्लों वाले गहरे भूरे \"निशाना\" धब्बे, पहले पुरानी, निचली पत्तियों पर",
                    "धब्बों के आसपास पत्ती का पीला पड़ना",
                    "फल के डंठल के पास धँसे हुए गहरे घाव भी हो सकते हैं",
                },
                Treatment = "निचली संक्रमित पत्तियाँ हटाएं, मिट्टी के छींटों से बचाव हेतु मल्च बिछाएं, फ़सल चक्र अपनाएं, और यदि धब्बे ऊपर फैलें तो फफूंदनाशक लगाएं।",
            },
            ["Tomato___Late_blight"] = new DiseaseInfo
            {
                Symptoms = new[]
                {
                    "पत्तियों पर बड़े, पानी-भीगे हरे-काले धब्बे जो तेज़ी से फैलते हैं",
                    "नम स्थितियों में पत्ती के नीचे सफ़ेद फफूंदी वृद्धि",
                    "फल पर भूरा, सख़्त-से-चिकना सड़न",
                },
                Treatment = "यह तेज़ी से फैलकर कुछ ही दिनों में पूरी फ़सल बर्बाद कर सकता है - संक्रमित पौधों को तुरंत हटाकर नष्ट करें, ओवरहेड सिंचाई से बचें, और ठंडे, नम मौसम में पहले से ही सुरक्षात्मक फफूंदनाशक लगाएं।",
            },
            ["Tomato___Leaf_Mold"] = new DiseaseInfo
            {
                Symptoms = new[]
                {
                    "पत्ती की ऊपरी सतह पर हल्के पीले धब्बे",
                    "ठीक नीचे पत्ती के निचले हिस्से में जैतूनी-हरे से धूसर-बैंगनी मखमली फफूंद",
                    "नम, कम हवादार ग्रीनहाउस/टनल में स्थिति बदतर होना",
                },
                Treatment = "पौधों के आसपास हवा का आवागमन बेहतर करें और नमी कम करें (उचित दूरी रखें, निचली पत्तियाँ काटें, ग्रीनहाउस हवादार करें), और समस्या बनी रहने पर अनुमोदित फफूंदनाशक लगाएं।",
            },
            ["Tomato___Septoria_leaf_spot"] = new DiseaseInfo
            {
                Symptoms = new[]
                {
                    "गहरे किनारे और तन/धूसर केंद्र वाले कई छोटे गोल धब्बे, आमतौर पर निचली पत्तियों से शुरू",
                    "धब्बों के केंद्र में छोटे काले बिंदु दिखना",
                    "अधिक धब्बों से पत्तियाँ पीली होकर गिरना",
                },
                Treatment = "संक्रमित निचली पत्तियाँ हटाएं, ओवरहेड सिंचाई से बचें, मिट्टी के छींटे कम करने हेतु मल्च बिछाएं, फ़सल चक्र अपनाएं, और फैलाव होने पर फफूंदनाशक लगाएं।",
            },
            ["Tomato___Spider_mites Two-spotted_spider_mite"] = new DiseaseInfo
            {
                Symptoms = new[]
                {
                    "पत्तियों पर बारीक पीले/सफ़ेद धब्बेदार निशान",
                    "अधिक प्रकोप में पत्ती के नीचे और तनों के बीच महीन जाला",
                    "पत्तियाँ कांस्य रंग की होकर सूखकर गिर सकती हैं",
                },
                Treatment = "पौधों पर तेज़ पानी की धार से माइट्स हटाएं, प्राकृतिक शिकारियों को बढ़ावा दें, और भारी प्रकोप में कीटनाशक साबुन या माइटनाशक का उपयोग करें - चूँकि यह फफूंद नहीं बल्कि माइट्स के कारण होता है, फफूंदनाशक इसका इलाज नहीं करते।",
            },
            ["Tomato___Target_Spot"] = new DiseaseInfo
            {
                Symptoms = new[]
                {
                    "पत्तियों, तनों और फल पर गोलाकार छल्लों वाले भूरे \"निशाना\" जैसे धब्बे",
                    "धब्बे मिलकर पत्तियों का व्यापक पीलापन और गिरना",
                    "नम स्थितियों में लक्षण बदतर होना",
                },
                Treatment = "छँटाई और उचित दूरी से हवा का आवागमन बेहतर करें, संक्रमित पत्तियाँ व मलबा हटाएं, फ़सल चक्र अपनाएं, और फैलाव होने पर अनुमोदित फफूंदनाशक लगाएं।",
            },
            ["Tomato___Tomato_mosaic_virus"] = new DiseaseInfo
            {
                Symptoms = new[]
                {
                    "पत्तियों पर हल्के और गहरे हरे धब्बेदार, मोज़ेक जैसा पैटर्न",
                    "पत्तियाँ मुड़ी हुई, संकरी, या फ़र्न जैसी हो सकती हैं",
                    "पौधे की वृद्धि रुक जाना और फल कम व कभी-कभी धब्बेदार होना",
                },
                Treatment = "इसका कोई इलाज नहीं है - फैलाव रोकने के लिए संक्रमित पौधों को हटाकर नष्ट करें, पौधों के बीच हाथ व औज़ार धोएं (यह वायरस स्पर्श से आसानी से फैलता है), और पौधों के पास तम्बाकू के उपयोग से बचें क्योंकि यह वायरस तम्बाकू उत्पादों में जीवित रह सकता है।",
            },
            ["Tomato___Tomato_Yellow_Leaf_Curl_Virus"] = new DiseaseInfo
            {
                Symptoms = new[]
                {
                    "पत्तियाँ ऊपर की ओर तेज़ी से मुड़कर पीली पड़ना, खासकर किनारों पर",
                    "पौधे स्पष्ट रूप से बौने और छोटे तनों वाले दिखना",
                    "फूल फल बनने से पहले ही गिर जाना",
                },
                Treatment = "इसका कोई इलाज नहीं है - संक्रमित पौधों को तुरंत हटाकर नष्ट करें, और इस वायरस को फैलाने वाली सफ़ेद मक्खी को चिपचिपे जाल, परावर्तक मल्च, या अनुमोदित कीटनाशक से नियंत्रित करें, क्योंकि कीट नियंत्रण ही फैलाव रोकने का मुख्य तरीका है।",
            },
        };

        private static readonly Dictionary<string, DiseaseInfo> Urdu = new()
        {
            ["Apple___Apple_scab"] = new DiseaseInfo
            {
                Symptoms = new[]
                {
                    "پتیوں پر زیتونی سبز سے بھورے مخملی دھبے جو بعد میں کھردرے ہو جاتے ہیں",
                    "پھل کی جلد پر بھی ایسے ہی گہرے، کھردرے دھبے",
                    "زیادہ متاثرہ پتیاں پیلی ہو کر جلد گر جاتی ہیں",
                },
                Treatment = "اگلے سیزن کے بیجانو کم کرنے کے لیے گری ہوئی پتیوں کو ہٹا کر تلف کریں، اور نم بہار کے موسم میں کلی کھلنے سے ہی ایپل اسکیب کے لیے منظور شدہ فنجی سائیڈ کا اسپرے کریں۔",
            },
            ["Apple___Black_rot"] = new DiseaseInfo
            {
                Symptoms = new[]
                {
                    "پتیوں پر جامنی کنارے والے بھورے دھبے (\"مینڈک آنکھ\" جیسے دھبے)",
                    "پھل پر دھنسے ہوئے، سیاہ کنارے والے زخم جو اندر سے سڑتے ہیں",
                    "شاخوں پر سرخی مائل بھورے کینکر",
                },
                Treatment = "کٹائی کے دوران کینکر زدہ لکڑی اور خشک شدہ پھلوں کو ہٹا کر تلف کریں، بڑھتے موسم میں فنجی سائیڈ لگائیں، اور قریبی مردہ لکڑی ہٹا دیں جو فنگس کو پناہ دیتی ہے۔",
            },
            ["Apple___Cedar_apple_rust"] = new DiseaseInfo
            {
                Symptoms = new[]
                {
                    "پتی کی اوپری سطح پر چمکدار پیلے نارنجی دھبے جو گرمیوں میں بڑے ہوتے جاتے ہیں",
                    "دھبوں کے اندر چھوٹے سیاہ نقطے نمودار ہونا",
                    "نم موسم میں پتی کے نیچے ٹیوب نما ساخت",
                },
                Treatment = "اگر ممکن ہو تو قریبی دیودار/جونیپر درخت ہٹا دیں (یہ اس فنگس کے زندگی چکر کا دوسرا حصہ ہیں)، اور گلابی کلی کے مرحلے سے لے کر گرمیوں کے آغاز تک حفاظتی فنجی سائیڈ لگائیں۔",
            },
            ["Cherry_(including_sour)___Powdery_mildew"] = new DiseaseInfo
            {
                Symptoms = new[]
                {
                    "پتیوں اور ٹہنیوں پر سفید پاؤڈر جیسے دھبے، اکثر نئی نشوونما پر شروع ہوتے ہیں",
                    "پتیوں کا مڑنا، سکڑنا، یا پیلا پڑنا",
                    "شدید صورتوں میں ٹہنی کی نشوونما رک جانا",
                },
                Treatment = "کٹائی کے ذریعے ہوا کی آمدورفت بہتر کریں، زیادہ نائٹروجن کھاد سے گریز کریں، اور سفید دھبے نظر آتے ہی سلفر پر مبنی یا دیگر منظور شدہ فنجی سائیڈ لگائیں۔",
            },
            ["Corn_(maize)___Cercospora_leaf_spot Gray_leaf_spot"] = new DiseaseInfo
            {
                Symptoms = new[]
                {
                    "پتی کی رگوں کے متوازی چھوٹے ہلکے بھورے سے سرمئی مستطیل دھبے",
                    "دھبے بڑھ کر آپس میں مل کر سرمئی بھورے حصے بن جانا",
                    "نم، گرم موسم اور زیادہ فصل کی باقیات ہونے پر علامات بدتر ہونا",
                },
                Treatment = "ایک سیزن کے لیے مکئی نہ لگائیں، پرانی فصل کی باقیات کو مٹی میں ملا دیں جہاں فنگس زندہ رہتی ہے، اور اگر حساس کھیتوں میں پھول آنے سے پہلے بیماری نظر آئے تو پتوں پر فنجی سائیڈ لگائیں۔",
            },
            ["Corn_(maize)___Common_rust_"] = new DiseaseInfo
            {
                Symptoms = new[]
                {
                    "پتی کی دونوں سطحوں پر چھوٹے، ابھرے ہوئے، دار چینی رنگ کے دانے دار دھبے",
                    "سیزن کے آخر تک دھبوں کا سیاہ پڑ جانا",
                    "شدید انفیکشن سے پتیاں پیلی ہو کر خشک ہو سکتی ہیں",
                },
                Treatment = "جہاں دستیاب ہو زنگ سے محفوظ اقسام لگائیں - فنجی سائیڈ کی ضرورت عام طور پر تب ہوتی ہے جب یہ بیماری جوان پودوں پر جلدی اور شدت سے ظاہر ہو۔",
            },
            ["Corn_(maize)___Northern_Leaf_Blight"] = new DiseaseInfo
            {
                Symptoms = new[]
                {
                    "پتی کے متوازی لمبے، سگار نما سرمئی سبز سے ہلکے بھورے زخم",
                    "علامات اکثر نیچے کی پرانی پتیوں سے شروع ہوتی ہیں",
                    "شدید انفیکشن میں زخم آپس میں مل کر پتی کے بڑے حصے کو ختم کر دیتے ہیں",
                },
                Treatment = "فصل کی گردش اپنائیں اور باقیات کو مٹی میں ملائیں، مزاحم اقسام منتخب کریں، اور اگر پھول آنے سے پہلے یا دوران اوپری پتیوں پر زخم نظر آئیں تو فنجی سائیڈ لگائیں۔",
            },
            ["Grape___Black_rot"] = new DiseaseInfo
            {
                Symptoms = new[]
                {
                    "پتیوں پر گہرے کناروں والے چھوٹے ہلکے بھورے دھبے",
                    "گچھے پر سیاہ، سکڑے ہوئے \"مومی\" دانے",
                    "متاثرہ پھل اور پتی کے دھبوں پر چھوٹے سیاہ فنگل نقطے نظر آنا",
                },
                Treatment = "کٹائی کے دوران خشک شدہ دانے اور متاثرہ بیلیں ہٹا دیں، چھتری میں ہوا کی آمدورفت بہتر کریں، اور نم موسموں میں کلی کھلنے سے لے کر پھل بننے تک فنجی سائیڈ پروگرام اپنائیں۔",
            },
            ["Grape___Esca_(Black_Measles)"] = new DiseaseInfo
            {
                Symptoms = new[]
                {
                    "پتی کی رگوں کے درمیان پیلی/سرخ دھاریوں کا \"شیر کی دھاری\" نمونہ",
                    "شدید صورتوں میں دانوں پر گہرے دھبے",
                    "جدید انفیکشن میں ٹہنیوں کا اچانک مرجھانا اور خشک ہونا",
                },
                Treatment = "ایک بار لکڑی متاثر ہونے کے بعد کوئی علاج نہیں ہے - نظر آنے والی علامات سے کافی نیچے تک متاثرہ لکڑی کاٹ کر تلف کریں، نم موسم میں کٹائی سے گریز کریں، اور کٹائی کے زخموں کی حفاظت کریں۔",
            },
            ["Grape___Leaf_blight_(Isariopsis_Leaf_Spot)"] = new DiseaseInfo
            {
                Symptoms = new[]
                {
                    "پتیوں پر زاویہ دار سرخی مائل بھورے دھبے",
                    "دھبے مل کر بڑے مردہ حصے بنانا",
                    "زیادہ دھبوں والی پتیاں پیلی ہو کر جلد گرنا، جس سے بیل کمزور ہوتی ہے",
                },
                Treatment = "کٹائی کے ذریعے چھتری میں ہوا کی آمدورفت بہتر کریں، گری ہوئی متاثرہ پتیاں ہٹائیں، اور نم ادوار میں منظور شدہ فنجی سائیڈ لگائیں۔",
            },
            ["Orange___Haunglongbing_(Citrus_greening)"] = new DiseaseInfo
            {
                Symptoms = new[]
                {
                    "پتیوں پر دھبے دار پیلاہٹ جو درمیانی رگ کے آر پار ہم آہنگ نہیں ہوتی",
                    "چھوٹے، بے ڈھنگے، کڑوے ذائقے والے پھل",
                    "وقت کے ساتھ ٹہنیوں کا خشک ہونا اور چھتری کا پتلا ہونا",
                },
                Treatment = "اس کا کوئی علاج نہیں ہے - پھیلاؤ روکنے کے لیے تصدیق شدہ متاثرہ درخت ہٹا کر تلف کریں، اور اس بیماری کو پھیلانے والے ایشیائی سٹرس سائلیڈ کیڑے کو تجویز کردہ کیڑے مار پروگرام سے کنٹرول کریں۔",
            },
            ["Peach___Bacterial_spot"] = new DiseaseInfo
            {
                Symptoms = new[]
                {
                    "پتیوں پر چھوٹے پانی سے بھرے دھبے جو جامنی سیاہ ہو کر اکثر گر جاتے ہیں، جس سے \"گولی کے سوراخ\" جیسی شکل بنتی ہے",
                    "پھل پر گہرے، دھنسے ہوئے دھبے",
                    "شدید انفیکشن میں جلد پتی گرنا",
                },
                Treatment = "ہوا کی آمدورفت کے لیے کٹائی کریں، اوپر سے آبپاشی سے گریز کریں جو پتیوں کے درمیان بیکٹیریا پھیلاتی ہے، اور غیرفعال موسم اور بڑھتے موسم کے آغاز میں تانبے پر مبنی بیکٹیریا کش دوا لگائیں۔",
            },
            ["Pepper,_bell___Bacterial_spot"] = new DiseaseInfo
            {
                Symptoms = new[]
                {
                    "پیلے ہالے والے چھوٹے گہرے، پانی سے بھرے دھبے پتیوں پر",
                    "دھبے مل کر پتیوں کے گرنے کا سبب بننا",
                    "پھل پر ابھرے، کھردرے دھبے",
                },
                Treatment = "بیماری سے پاک بیج/پنیری استعمال کریں، گیلے کھیت میں کام کرنے سے گریز کریں، فصل کی گردش اپنائیں، اور دھبے نظر آتے ہی تانبے پر مبنی بیکٹیریا کش دوا لگائیں۔",
            },
            ["Potato___Early_blight"] = new DiseaseInfo
            {
                Symptoms = new[]
                {
                    "گول حلقوں والے گہرے بھورے دھبے (\"ہدف\" نمونہ)، پہلے پرانی، نچلی پتیوں پر",
                    "دھبوں کے ارد گرد پتی کے بافت کا پیلا پڑنا",
                    "کند پر گہرے، دھنسے ہوئے زخم بھی ہو سکتے ہیں",
                },
                Treatment = "آلو اور ٹماٹر سے فصل کی گردش بدلیں، متاثرہ پتیاں ہٹائیں، پودوں کو اچھی طرح کھلائیں (دباؤ والے پودے زیادہ حساس ہوتے ہیں)، اور اگر سیزن کے آغاز میں دھبے نظر آئیں تو فنجی سائیڈ لگائیں۔",
            },
            ["Potato___Late_blight"] = new DiseaseInfo
            {
                Symptoms = new[]
                {
                    "پتیوں پر پانی سے بھرے ہلکے سے گہرے سبز دھبے جو بھورے/سیاہ ہو کر تیزی سے پھیلتے ہیں",
                    "نم موسم میں پتی کے نیچے سفید فنگل تہہ",
                    "چند دنوں میں پورا پودا سڑ سکتا ہے",
                },
                Treatment = "یہ بہت تیزی سے پھیلتا ہے - متاثرہ پودوں کو فوراً ہٹا کر تلف کریں، اوپر سے آبپاشی سے گریز کریں، اور ٹھنڈے، نم موسم میں پہلے سے ہی حفاظتی فنجی سائیڈ لگائیں۔",
            },
            ["Squash___Powdery_mildew"] = new DiseaseInfo
            {
                Symptoms = new[]
                {
                    "پتی کی اوپری اور نچلی سطحوں اور تنوں پر سفید پاؤڈر جیسے دھبے",
                    "علامات عام طور پر پرانی پتیوں پر پہلے شروع ہوتی ہیں",
                    "پھپھوندی پھیلنے کے ساتھ پتیاں پیلی ہو کر مڑتی اور خشک ہوتی ہیں",
                },
                Treatment = "ہوا کی آمدورفت کے لیے مکمل دھوپ اور مناسب فاصلے پر لگائیں، زیادہ متاثرہ پتیاں ہٹائیں، اور علامات نظر آتے ہی سلفر پر مبنی یا دیگر منظور شدہ فنجی سائیڈ لگائیں۔",
            },
            ["Strawberry___Leaf_scorch"] = new DiseaseInfo
            {
                Symptoms = new[]
                {
                    "پتیوں پر چھوٹے گہرے جامنی دھبے جو بڑھ کر آپس میں مل جاتے ہیں",
                    "پتیاں جھلسی ہوئی، سرخی مائل بھوری نظر آنے لگتی ہیں",
                    "شدید انفیکشن میں پودے کی طاقت اور پیداوار کم ہونا",
                },
                Treatment = "کٹائی کے بعد پرانی اور متاثرہ پتیاں ہٹائیں، اوپر سے آبپاشی سے گریز کریں، ہوا کی آمدورفت کے لیے مناسب فاصلہ رکھیں، اور اگر بیماری بار بار ہو تو منظور شدہ فنجی سائیڈ لگائیں۔",
            },
            ["Tomato___Bacterial_spot"] = new DiseaseInfo
            {
                Symptoms = new[]
                {
                    "پتیوں پر چھوٹے گہرے، چکنے نظر آنے والے دھبے، اکثر پیلے ہالے کے ساتھ",
                    "پھل پر ابھرے، کھردرے زخم",
                    "شدید انفیکشن سے پتیاں گرنا",
                },
                Treatment = "بیماری سے پاک بیج/پنیری استعمال کریں، اوپر سے آبپاشی اور گیلے پودوں کے ساتھ کام کرنے سے گریز کریں، فصل کی گردش اپنائیں، اور علامات نظر آتے ہی تانبے پر مبنی بیکٹیریا کش دوا لگائیں۔",
            },
            ["Tomato___Early_blight"] = new DiseaseInfo
            {
                Symptoms = new[]
                {
                    "گول حلقوں والے گہرے بھورے \"ہدف\" دھبے، پہلے پرانی، نچلی پتیوں پر",
                    "دھبوں کے ارد گرد پتی کا پیلا پڑنا",
                    "پھل کے ڈنٹھل کے قریب دھنسے ہوئے گہرے زخم بھی ہو سکتے ہیں",
                },
                Treatment = "نچلی متاثرہ پتیاں ہٹائیں، مٹی کے چھینٹوں سے بچاؤ کے لیے ملچ بچھائیں، فصل کی گردش اپنائیں، اور اگر دھبے اوپر پھیلیں تو فنجی سائیڈ لگائیں۔",
            },
            ["Tomato___Late_blight"] = new DiseaseInfo
            {
                Symptoms = new[]
                {
                    "پتیوں پر بڑے، پانی سے بھرے سبز سیاہ دھبے جو تیزی سے پھیلتے ہیں",
                    "نم حالات میں پتی کے نیچے سفید فنگل نشوونما",
                    "پھل پر بھورا، سخت سے چکنا سڑاؤ",
                },
                Treatment = "یہ تیزی سے پھیل کر چند دنوں میں پوری فصل برباد کر سکتا ہے - متاثرہ پودوں کو فوراً ہٹا کر تلف کریں، اوپر سے آبپاشی سے گریز کریں، اور ٹھنڈے، نم موسم میں پہلے سے ہی حفاظتی فنجی سائیڈ لگائیں۔",
            },
            ["Tomato___Leaf_Mold"] = new DiseaseInfo
            {
                Symptoms = new[]
                {
                    "پتی کی اوپری سطح پر ہلکے پیلے دھبے",
                    "بالکل نیچے پتی کے نچلے حصے میں زیتونی سبز سے سرمئی جامنی مخملی پھپھوندی",
                    "نم، کم ہوادار گرین ہاؤسز/سرنگوں میں حالت بدتر ہونا",
                },
                Treatment = "پودوں کے ارد گرد ہوا کی آمدورفت بہتر کریں اور نمی کم کریں (مناسب فاصلہ رکھیں، نچلی پتیاں کاٹیں، گرین ہاؤس ہوادار کریں)، اور مسئلہ برقرار رہنے پر منظور شدہ فنجی سائیڈ لگائیں۔",
            },
            ["Tomato___Septoria_leaf_spot"] = new DiseaseInfo
            {
                Symptoms = new[]
                {
                    "گہرے کناروں اور ہلکے بھورے/سرمئی مرکز والے کئی چھوٹے گول دھبے، عام طور پر نچلی پتیوں سے شروع",
                    "دھبوں کے مرکز میں چھوٹے سیاہ نقطے نظر آنا",
                    "زیادہ دھبوں سے پتیاں پیلی ہو کر گرنا",
                },
                Treatment = "متاثرہ نچلی پتیاں ہٹائیں، اوپر سے آبپاشی سے گریز کریں، مٹی کے چھینٹے کم کرنے کے لیے ملچ بچھائیں، فصل کی گردش اپنائیں، اور پھیلاؤ ہونے پر فنجی سائیڈ لگائیں۔",
            },
            ["Tomato___Spider_mites Two-spotted_spider_mite"] = new DiseaseInfo
            {
                Symptoms = new[]
                {
                    "پتیوں پر باریک پیلے/سفید دھبے دار نشانات",
                    "شدید حملے میں پتی کے نیچے اور تنوں کے درمیان باریک جالا",
                    "پتیاں تانبے رنگ کی ہو کر خشک ہو کر گر سکتی ہیں",
                },
                Treatment = "پودوں پر تیز پانی کی دھار سے مائٹس ہٹائیں، قدرتی شکاریوں کی حوصلہ افزائی کریں، اور شدید حملے میں کیڑے مار صابن یا مائٹ کش دوا استعمال کریں - چونکہ یہ فنگس نہیں بلکہ مائٹس کی وجہ سے ہوتا ہے، فنجی سائیڈ اس کا علاج نہیں کرتیں۔",
            },
            ["Tomato___Target_Spot"] = new DiseaseInfo
            {
                Symptoms = new[]
                {
                    "پتیوں، تنوں اور پھل پر گول حلقوں والے بھورے \"ہدف\" جیسے دھبے",
                    "دھبے مل کر پتیوں کی وسیع پیلاہٹ اور گرنا",
                    "نم حالات میں علامات بدتر ہونا",
                },
                Treatment = "کٹائی اور مناسب فاصلے سے ہوا کی آمدورفت بہتر کریں، متاثرہ پتیاں اور ملبہ ہٹائیں، فصل کی گردش اپنائیں، اور پھیلاؤ ہونے پر منظور شدہ فنجی سائیڈ لگائیں۔",
            },
            ["Tomato___Tomato_mosaic_virus"] = new DiseaseInfo
            {
                Symptoms = new[]
                {
                    "پتیوں پر ہلکے اور گہرے سبز دھبے دار، موزیک جیسا نمونہ",
                    "پتیاں مڑی ہوئی، تنگ، یا فرن جیسی ہو سکتی ہیں",
                    "پودے کی نشوونما رک جانا اور پھل کم اور کبھی کبھار دھبے دار ہونا",
                },
                Treatment = "اس کا کوئی علاج نہیں ہے - پھیلاؤ روکنے کے لیے متاثرہ پودوں کو ہٹا کر تلف کریں، پودوں کے درمیان ہاتھ اور اوزار دھوئیں (یہ وائرس چھونے سے آسانی سے پھیلتا ہے)، اور پودوں کے قریب تمباکو کے استعمال سے گریز کریں کیونکہ یہ وائرس تمباکو کی مصنوعات میں زندہ رہ سکتا ہے۔",
            },
            ["Tomato___Tomato_Yellow_Leaf_Curl_Virus"] = new DiseaseInfo
            {
                Symptoms = new[]
                {
                    "پتیاں تیزی سے اوپر کی طرف مڑ کر پیلی پڑنا، خاص طور پر کناروں پر",
                    "پودے واضح طور پر بونے اور چھوٹے تنوں والے نظر آنا",
                    "پھول پھل بننے سے پہلے ہی گر جانا",
                },
                Treatment = "اس کا کوئی علاج نہیں ہے - متاثرہ پودوں کو فوراً ہٹا کر تلف کریں، اور اس وائرس کو پھیلانے والی سفید مکھی کو چپچپے جال، عکاسی کرنے والی ملچ، یا منظور شدہ کیڑے مار دوا سے کنٹرول کریں، کیونکہ کیڑے کا کنٹرول ہی پھیلاؤ روکنے کا اہم طریقہ ہے۔",
            },
        };
    }
}
