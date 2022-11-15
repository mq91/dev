import http
from typing import Union

from fastapi import FastAPI, Request
from pydantic import BaseModel

app = FastAPI()


@app.get("/")
def read_root():
    return {"Hello":"World"}


class Item(BaseModel):
    name: str
    description: str | None = None
    price: float
    tax: float | None = None

# @app.get("/items/{item_id}")
# def read_item(item_id: int, q: Union[str, None] = None):
#     return {"item_id": item_id, "q": q}

@app.post("/items/")
async def create_item(item: Item):
    return item