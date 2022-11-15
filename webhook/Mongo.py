from pymongo import MongoClient

client = MongoClient("mongodb+srv://mohammed:Mm971839@cluster0.wspaf.mongodb.net/?retryWrites=true&w=majority")

db = client["webhook"]

example = {
    "ID":1,
    "Title": "test",
    "Name":{
        "First":"Mohammed",
        "Seconed":"Abdullah",
        "Last":"AlQarni"
    },
    "age": 30
}

DBMoyasar = db["Moyasar"]

DBMoyasar.insert_one(example)
