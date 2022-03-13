FROM python:3.9

WORKDIR /app

COPY requirements.txt .

RUN pip install -U pip
RUN pip install uWSGI
RUN pip install -r requirements.txt

COPY . .

ENV FLASK_ENV=production
ENV PYTHONDONTWRITEBYTECODE 1
ENV PYTHONUNBUFFERED 1

CMD ["uwsgi", "app.ini"]